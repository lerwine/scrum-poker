Param(
    [int]$PortNumber = 8080,
    [string]$AdminUserName = 'admin'
)

<#
Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process
#>
class SessionUser {
    [string]$UserName;
    [byte[]]$Token;
}

Set-Variable -Name 'CryptoServiceProvider' -Option Constant -Scope 'Script' -Value([System.Security.Cryptography.RNGCryptoServiceProvider]::new());
Function Get-RandomTokenBytes
{
    [CmdletBinding()]
    [OutputType([string[]])]
    Param()
    $Data = New-Object -TypeName 'System.Byte[]' -ArgumentList 64;
    $Script:CryptoServiceProvider.GetBytes($Data);
    return $Data;
}
Set-Variable -Name 'SessionToken' -Option Constant -Scope 'Script' -Value (Get-RandomTokenBytes);
Set-Variable -Name 'SessionUsers' -Option Constant -Scope 'Script' -Value ([System.Collections.ObjectModel.Collection[SessionUser]]::new());

Function Get-SessionUser {
    [CmdletBinding()]
    [OutputType([SessionUser])]
    Param(
        [Parameter(Mandatory = $true)]
        [string]$UserName
    )
    foreach ($u in $Script:SessionUsers) {
        if ($u.UserName -ieq $UserName) {
            return $u;
        }
    }
}

Function New-SessionUser {
    [CmdletBinding()]
    [OutputType([SessionUser])]
    Param(
        [Parameter(Mandatory = $true)]
        [string]$UserName
    )

    $Result = [SessionUser]@{
        UserName = $UserName;
        Token = Get-RandomTokenBytes;
    }
    $Script:SessionUsers.Add($Result);
    return $Result;
}

Set-Variable -Name 'AdminUser' -Option Constant -Scope 'Script' -Value (New-SessionUser -UserName $AdminUserName);
Set-Variable -Name 'UTF8Encoding' -Option Constant -Scope 'Script' -Value ([System.Text.UTF8Encoding]::new($false, $false));

Function Get-TokenString {
    [CmdletBinding()]
    [OutputType([string])]
    Param(
        [Parameter(Mandatory = $true)]
        [SessionUser]$User
    )
    [byte[]]$Data = $Script:SessionToken + $User.Token;
    return [System.Convert]::ToBase64String([System.Security.Cryptography.ProtectedData]::Protect($Data, $Script:UTF8Encoding.GetBytes($User.UserName), [System.Security.Cryptography.DataProtectionScope]::CurrentUser));
}

Set-Variable -Name 'CookieName_AuthenticationToken' -Option Constant -Scope 'Script' -Value 'AuthenticationToken';
Function Get-AuthenticatedUser {
    [CmdletBinding()]
    [OutputType([SessionUser])]
    Param(
        [Parameter(Mandatory = $true)]
        [System.Net.HttpListenerRequest]$Request
    )
    [System.Net.Cookie]$Cookie = $null;
    foreach ($c in $Request.Cookies) {
        if ($c.Name -eq $Script:CookieName_AuthenticationToken) {
            $Cookie = $c;
            break;
        }
    }
    if ($null -eq $c) { return $null }
    ($UserName, $TokenString) = $Cookie.Value.Split(' ', 2);
    if ([string]::IsNullOrWhiteSpace($UserName) -or [string]::IsNullOrWhiteSpace($TokenString)) { return $null }
    [SessionUser]$User = Get-SessionUser -UserName $UserName;
    if ($null -eq $User) { return $null }
    [byte[]]$Data = [System.Convert]::FromBase64String($TokenString);
    if ($null -eq $Data -or $Data.Length -ne 128) { return $null }
    $Data = [System.Security.Cryptography.ProtectedData]::Unprotect($Data, [SessionUser]::Encoding.GetBytes($User.UserName), [System.Security.Cryptography.DataProtectionScope]::CurrentUser);
    $l1 = $Script:SessionToken.Length;
    $l2 = $l1 = $User.Token.Length;
    if ($Data.Length -ne $l2) { return $null }
    for ($i = 0; $i -lt $l1; $i++) {
        if ($Data[$i] -ne $Script:SessionToken[$i]) { return $null }
    }
    for ($i = $l1; $i -lt $l2; $i++) {
        if ($Data[$i] -ne $User.Token[$i - $l1]) { return $null }
    }
    return $User;
}


Function Write-PlainTextResponse {
    [CmdletBinding()]
    [OutputType([SessionUser])]
    Param(
        [Parameter(Mandatory = $true)]
        [System.Net.HttpListenerResponse]$Response,

        [string]$Message
    )
    $Buffer = $Script:UTF8Encoding.GetBytes($Message);
    $Response.ContentType = 'text/plain';
    $Response.ContentLength64 = $Buffer.Length;
    $OutputStream = $Response.OutputStream;
    $OutputStream.Write($Buffer, 0, $Buffer.Length);
    $OutputStream.Flush();
    $OutputStream.Close();
}

Function New-XhtmlHeadElement {
    [CmdletBinding()]
    [OutputType([System.Xml.Linq.XElement])]
    Param(
        [Parameter(Mandatory = $true)]
        [string]$Title
    )

    return [System.Xml.Linq.XElement]::new('head',
        [System.Xml.Linq.XElement]::new('title', $Title)
    )
}

Function Get-XHtmlDocument {
    [CmdletBinding()]
    [OutputType([System.Xml.Linq.XDocument])]
    Param(
        [Parameter(Mandatory = $true)]
        [System.Xml.Linq.XContainer]$Html
    )
    [System.Xml.Linq.XDocument]$XDocument = $null;
    [System.Xml.Linq.XElement]$HeadElement = $null;
    [System.Xml.Linq.XElement]$BodyElement = $null;
    if ($Html -is [System.Xml.Linq.XDocument]) {
        $XDocument = $Html | Write-Output;
    } else {
        if ($null -ne $Html.Document) {
            $Html.Document | Write-Output;
        } else {
            while ($null -ne $Html.Parent) {
                $Html = $Html.Parent;
            }
            
            if ($Html.Name -eq 'head' -or $Html.Name -eq 'body') {
                [System.Xml.Linq.XDocument]::new(
                    [System.Xml.Linq.XElement]::new('head')
                )
            } else {
            }
            switch ($Html.Name) {
                'html' {
                    return [System.Xml.Linq.XDocument]::new($Html);
                }
                'head' {
                    return [System.Xml.Linq.XDocument]::new(
                        [System.Xml.Linq.XElement]::new(
                            $Html,
                            [System.Xml.Linq.XElement]::new('body')
                        )
                    );
                }
                'body' {
                    return [System.Xml.Linq.XDocument]::new(
                        [System.Xml.Linq.XElement]::new(
                            [System.Xml.Linq.XElement]::new('head'),
                            $Html
                        )
                    );
                }
            }
        }
    }
}

Function Write-XhtmlResponse {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true)]
        [System.Net.HttpListenerResponse]$Response,
        
        [string]$Title, 
        
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [System.Xml.Linq.XNode[]]$BodyNodes,
        
        [SessionUser]$User
    )

    Begin {
        $BodyElement = [System.Xml.Linq.XElement]::new('body');
        $Html = [System.Xml.Linq.XDocument]::new(
            [System.Xml.Linq.XElement]::new('html', [System.Xml.Linq.XAttribute]::new('lang', 'en'),
                [System.Xml.Linq.XElement]::new('head',
                    [System.Xml.Linq.XElement]::new('meta', [System.Xml.Linq.XAttribute]::new('charset', 'utf-8')),
                    [System.Xml.Linq.XElement]::new('title', "Scrum Poker - $Title"),
                    [System.Xml.Linq.XElement]::new('base', [System.Xml.Linq.XAttribute]::new('href', '/')),
                    [System.Xml.Linq.XElement]::new('meta', [System.Xml.Linq.XAttribute]::new('name', 'viewport'), [System.Xml.Linq.XAttribute]::new('content', 'width=1024, initial-scale=1')),
                    [System.Xml.Linq.XElement]::new('link', [System.Xml.Linq.XAttribute]::new('rel', 'icon'), [System.Xml.Linq.XAttribute]::new('type', 'image/x-icon'), [System.Xml.Linq.XAttribute]::new('href', 'favicon.ico'))
                ),
                $BodyElement
            )
        );
    }

    Process {
        $BodyNodes | ForEach-Object { $BodyElement.Add($_) }
    }

    End {
        if ($PSBoundParameters.ContainsKey('User')) {
            $Cookie = [System.Net.Cookie]::new($Script:CookieName_AuthenticationToken, (Get-TokenString -User $User));

        }
        $Buffer = $Script:UTF8Encoding.GetBytes($Html.ToString([System.Xml.Linq.SaveOptions]::None));
        $Response.ContentType = 'text/html';
        $Response.ContentLength64 = $Buffer.Length;
        $OutputStream = $Response.OutputStream;
        $OutputStream.Write($Buffer, 0, $Buffer.Length);
        $OutputStream.Flush();
        $OutputStream.Close();
    }
}


Function Write-NotFoundResponse {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true)]
        [System.Net.HttpListenerResponse]$Response,
        
        [Uri]$Uri,
        
        [SessionUser]$User
    )
    
    $Response.StatusCode = 404;
    $Response.StatusDescription = "Resource $($Uri.PathAndQuery) not found.";
    $Buffer = $Script:UTF8Encoding.GetBytes(([System.Xml.Linq.XDocument]::new(
        [System.Xml.Linq.XElement]::new('html', [System.Xml.Linq.XAttribute]::new('lang', 'en'),
            [System.Xml.Linq.XElement]::new('head',
                [System.Xml.Linq.XElement]::new('meta', [System.Xml.Linq.XAttribute]::new('charset', 'utf-8')),
                [System.Xml.Linq.XElement]::new('title', 'Scrum Poker - Resource not found'),
                [System.Xml.Linq.XElement]::new('base', [System.Xml.Linq.XAttribute]::new('href', '/')),
                [System.Xml.Linq.XElement]::new('meta', [System.Xml.Linq.XAttribute]::new('name', 'viewport'), [System.Xml.Linq.XAttribute]::new('content', 'width=1024, initial-scale=1')),
                [System.Xml.Linq.XElement]::new('link', [System.Xml.Linq.XAttribute]::new('rel', 'icon'), [System.Xml.Linq.XAttribute]::new('type', 'image/x-icon'), [System.Xml.Linq.XAttribute]::new('href', 'favicon.ico'))
            ),
            [System.Xml.Linq.XElement]::new('body',
                [System.Xml.Linq.XElement]::new('h1', 'Resource not found'),
                "$($Uri.PathAndQuery) was not found."
            )
        )
    )).ToString([System.Xml.Linq.SaveOptions]::None));
    $Response.ContentType = 'text/html';
    $Response.ContentLength64 = $Buffer.Length;
    $OutputStream = $Response.OutputStream;
    $OutputStream.Write($Buffer, 0, $Buffer.Length);
    $OutputStream.Flush();
    $OutputStream.Close();
}

Function Write-LogiFormResponse {
    [CmdletBinding()]
    [OutputType([SessionUser])]
    Param(
        [Parameter(Mandatory = $true)]
        [System.Net.HttpListenerResponse]$Response,

        [string]$ReturnUrl
    )
    $FormElement = [System.Xml.Linq.XElement]::new('form',
        [System.Xml.Linq.XElement]::new('div', [System.Xml.Linq.XAttribute]::new('style', 'font-weight: bold'), 'User Name'),
        [System.Xml.Linq.XElement]::new('div',
            [System.Xml.Linq.XElement]::new('input', [System.Xml.Linq.XAttribute]::new('type', 'text'), [System.Xml.Linq.XAttribute]::new('name', 'UserNameTextBox'), [System.Xml.Linq.XAttribute]::new('id', 'UserNameTextBox'), [System.Xml.Linq.XAttribute]::new('style', 'width: 250px'))
        ),
        [System.Xml.Linq.XElement]::new('div', [System.Xml.Linq.XAttribute]::new('style', 'font-weight: bold'), 'Token String'),
        [System.Xml.Linq.XElement]::new('div',
            [System.Xml.Linq.XElement]::new('input', [System.Xml.Linq.XAttribute]::new('type', 'password'), [System.Xml.Linq.XAttribute]::new('name', 'TokenTextBox'), [System.Xml.Linq.XAttribute]::new('id', 'TokenTextBox'), [System.Xml.Linq.XAttribute]::new('style', 'width: 100%'))
        )
    );
    if ($PSBoundParameters.ContainsKey('ReturnUrl')) {
        $FormElement.Add(([System.Xml.Linq.XElement]::new('input', [System.Xml.Linq.XAttribute]::new('type', 'hidden'), [System.Xml.Linq.XAttribute]::new('name', 'ReturnUrl'), [System.Xml.Linq.XAttribute]::new('value', $ReturnUrl))));
    }
    $FormElement.Add(([System.Xml.Linq.XElement]::new('div',
        [System.Xml.Linq.XElement]::new('input', [System.Xml.Linq.XAttribute]::new('type', 'submit'), [System.Xml.Linq.XAttribute]::new('value', 'Log In'))
    )));
    (
        [System.Xml.Linq.XElement]::new('h1', 'Scrum Poker Login'),
        $FormElement
    ) | Write-XhtmlResponse -Response $Response -Title 'Login';
}

Function Get-LocalFilePath {
    [CmdletBinding()]
    [OutputType([string])]
    Param(
        [string]$Path
    )

    $LocalPath = "MyPowerShellSite:$Path";
    if (Test-Path -LiteralPath $LocalPath -PathType Leaf) { return $LocalPath }
    if (Test-Path -LiteralPath $LocalPath -PathType Container) {
        foreach ($FileName in @('index.html', 'index.htm', 'default.html', 'default.htm')) {
            $LocalPath = "MyPowerShellSite:$Path" | Join-Path -ChildPath $FileName;
            if (Test-Path -LiteralPath $LocalPath -PathType Leaf) { return $LocalPath }
        }
    }
}

#<#
$HttpListener = [System.Net.HttpListener]::new();
$UriBuilder = [System.UriBuilder]::new('http://localhost');
$UriBuilder.Port = $PortNumber;
$HttpListener.Prefixes.Add($UriBuilder.Uri.AbsoluteUri);
if ($null -eq (Get-PSDrive -LiteralName 'ScrumPokerSite' -ErrorAction SilentlyContinue)) {
    (New-PSDrive -Name 'ScrumPokerSite' -PSProvider FileSystem -Root ($PSScriptRoot | Join-Path -ChildPath 'WebRoot')) | Out-Null;
}
$HttpListener.Start();
"Listening on $($UriBuilder.Uri.AbsoluteUri)" | Write-Host;
$AdminTokenString = Get-TokenString -User $Script:AdminUser;
"Administrative token is $([Uri]::EscapeDataString($AdminTokenString))" | Write-Host;
[System.IO.File]::WriteAllLines(($PSScriptRoot | Join-Path -ChildPath 'SessionInfo.txt'), ([string[]]@(
    $UriBuilder.Uri.AbsoluteUri,
    $AdminUserName,
    $AdminTokenString
)), $Script:UTF8Encoding)
try {
    # https://learn.microsoft.com/en-us/dotnet/api/system.net.httplistenercontext
    $IsRunnning = $true;
    [System.Net.HttpListenerContext]$Context = $null;
    dp {
        if ($null -eq ($Context = $HttpListener.GetContext())) { break }
        # https://learn.microsoft.com/en-us/dotnet/api/system.net.httplistenerrequest
        [System.Net.HttpListenerRequest]$Request = $Context.Request;
        # https://learn.microsoft.com/en-us/dotnet/api/system.net.httplistenerresponse
        [System.Net.HttpListenerResponse]$Response = $Context.Response;
        $LocalPath =$Request.LocalPath;
        switch ($Request.HttpMethod) {
            'GET' {
                switch ($LocalPath) {
                    '/quit' {
                        [SessionUser]$User = $null;
                        $User = Get-AuthenticatedUser -Request $Request;
                        if ($null -eq $User) {
                            Write-LogiFormResponse -Response $Context.Response -ReturnUrl = $Url.PathAndQuery;
                        } else {
                            $LocalPath = Get-LocalFilePath -Path $LocalPath;
                            if ([object]::ReferenceEquals($Script:AdminUser, $User)) {
                                (
                                    [System.Xml.Linq.XElement]::new('h1', 'Exit'),
                                    'Exiting Web Application'
                                ) | Write-XhtmlResponse -Response $Response -Title 'Exiting Web Application';
                                $IsRunnning = $false;
                            } else {
                                Write-NotFoundResponse $Request.Url;
                            }
                        }
                        break;
                    }
                    '/login' {
                        Write-LogiFormResponse -Response $Context.Response;
                        break;
                    }
                    default {
                        [SessionUser]$User = $null;
                        $User = Get-AuthenticatedUser -Request $Request;
                        if ($null -eq $User) {
                            Write-LogiFormResponse -Response $Context.Response -ReturnUrl = $Url.PathAndQuery;
                        } else {
                            $LocalPath = Get-LocalFilePath -Url $Request.Url;
                            if ($null -eq $LocalPath) {
                                Write-NotFoundResponse $Request.Url;
                            } else {
                                $Content = Get-Content -Encoding Byte -LiteralPath $LocalPath;
                                $Response.ContentType = [System.Web.MimeMapping]::GetMimeMapping($LocalPath);
                                $Response.ContentLength64 = $Content.Length;
                                Write-Information -MessageData "Sending $($Response.ContentLength64) bytes ($($Response.ContentType)) from $LocalPath" -InformationAction Continue;
                                $Response.OutputStream.Write($Content, 0, $Content.Length);
                                $Response.Close();
                            }
                        }
                        break;
                    }
                }
                break;
            }
            'POST' {
                if ($LocalPath -eq '/login') {
                    $Request.ContentEncoding;
                    $Request.ContentLength64;
                    $Request.ContentType;
                } else {
                    switch ($LocalPath) {
                        '/login' {
                            break;
                        }
                    }
                }
                break;
            }
        }
    } while ($IsRunnning);
} finally { $HttpListener.Stop(); }

#>