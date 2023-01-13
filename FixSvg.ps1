$XmlWriterSettings = [System.Xml.XmlWriterSettings]@{
  Indent = $true;
  Encoding = [System.Text.UTF8Encoding]::new($false, $false);
  OmitXmlDeclaration = $true;
};

foreach ($File in (Get-ChildItem -FIlter '*.svg' -Path ($PSScriptRoot | Join-Path -ChildPath 'src\assets'))) {
  [Xml]$Xml = '<svg/>';
  $Xml.Load($File.FullName);
  $Nsmgr = [System.Xml.XmlNamespaceManager]::new($Xml.NameTable);
  $Nsmgr.AddNamespace("s", "http://www.w3.org/2000/svg");
  $Nsmgr.AddNamespace("l", "http://www.w3.org/1999/xlink");
  $Nsmgr.AddNamespace("h", "http://www.w3.org/1999/xhtml");
  [System.Xml.XmlElement[]]$ParentGroupElements = $Xml.SelectNodes('/s:svg/s:g/s:g[not(count(s:switch)=0)]', $Nsmgr);
  if ($ParentGroupElements.Length -eq 0) {
    Write-Warning -Message "$($File.Name): No inner group element";
  } else {
    $IsChanged = $false;
    #"$($File.Name): $($ParentGroupElements.Length) inner group elements" | Write-Host;
    foreach ($Pge in $ParentGroupElements) {
      [System.Xml.XmlElement[]]$TextElements = $Pge.SelectNodes("s:switch/s:text", $Nsmgr);
      if ($TextElements.Length -gt 1) {
        "$($File.Name): Has $$TextElements.Length() Text Nodes" | Write-Warning;
      } else {
        if ($TextElements.Length -eq 1) {
          $Attr = $Pge.SelectSingleNode('@transform');
          $Te = $TextElements[0];
          $t = $Te.InnerText.Trim();
          if (-not $t.Contains(' ')) {
            if ($null -ne $Attr) {
              $Te.Attributes.Append($Xml.CreateAttribute('transform')).Value = $Attr.Value;
            }
            $Te.ParentNode.RemoveChild($Te) | Out-Null;
            $Pge.ParentNode.InsertAfter($Te, $Pge) | Out-Null;
            $Pge.ParentNode.RemoveChild($Pge) | Out-Null;
            $IsChanged = $true;
          }
        }
      }
    }
    if ($IsChanged) {
        $XmlWriter = [System.Xml.XmlWriter]::Create($File.FullName, $XmlWriterSettings);
        try {
            $Xml.WriteTo($XmlWriter);
            $XmlWriter.Flush();
        } finally { $XmlWriter.Close(); }
    }
  }
  # if ($Changed) {
  #   $XmlWriter = [System.Xml.XmlWriter]::Create($File.FullName, $XmlWriterSettings);
  #   try {
  #       $Xml.WriteTo($XmlWriter);
  #       $XmlWriter.Flush();
  #   } finally { $XmlWriter.Close(); }
  # }
}
