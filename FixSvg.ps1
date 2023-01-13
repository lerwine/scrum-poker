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
    foreach ($TextElement in $Xml.DocumentElement.SelectSingleNode('s:g[1]/s:g[1]/s:switch/s:text', $Nsmgr)) {
      if (-not $TextElement.InnerText.Trim().Contains(' ')) {
        $TextElement.ParentNode.RemoveChild($TextElement) | Out-Null;
        $ToRemove = $TextElement.ParentNode.ParentNode;
        $ToRemove.ParentNode.InsertAfter($TextElement, $ToRemove) | Out-Null;
        $ToRemove.ParentNode.RemoveChild($ToRemove) | Out-Null;
        $Changed = $true;
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
