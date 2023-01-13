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
    $Changed = $false;
    foreach ($GroupParent in $Xml.DocumentElement.SelectNodes('s:g', $Nsmgr))
    {
      foreach ($GroupElement in $GroupParent.SelectNodes('s:g', $Nsmgr))
      {
        $FoundText = $false;
        foreach ($TextElement in $GroupElement.SelectNodes('s:switch[1]/s:text[1]', $Nsmgr)) {
          if (-not $TextElement.InnerText.Trim().Contains(' ')) {
              $TextElement.ParentNode.RemoveChild($TextElement) | Out-Null;
              $GroupParent.InsertAfter($TextElement, $GroupElement) | Out-Null;
              $FoundText = $true;
              break;
          }
        }
        if ($FoundText) {
          $Changed = $true;
          $GroupParent.RemoveChild($GroupElement) | Out-Null;
          break;
        }
      }
    }
    if ($Changed) {
      $XmlWriter = [System.Xml.XmlWriter]::Create($File.FullName, $XmlWriterSettings);
      try {
          $Xml.WriteTo($XmlWriter);
          $XmlWriter.Flush();
      } finally { $XmlWriter.Close(); }
    }
}
