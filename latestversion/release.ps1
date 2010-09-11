$dir = resolve-path .\

function Extract-Package
{
	param([string]$zipfilename, [string] $destination)

	if(test-path($zipfilename))
	{	
		$shellApplication = new-object -com shell.application
		$zipPackage = $shellApplication.NameSpace($zipfilename)
		$destinationFolder = $shellApplication.NameSpace($destination)
		$destinationFolder.CopyHere($zipPackage.Items())
	}
}

function DownloadFiles {
    remove-item "MVCContrib.Extras.release.zip" -ErrorAction:SilentlyContinue
    remove-item "MVCContrib.release.zip"  -ErrorAction:SilentlyContinue
    remove-item "MVCContrib.source.zip"  -ErrorAction:SilentlyContinue
    $extrasUrl  = "http://build-oss.headspringlabs.com/guestAuth/repository/download/bt2/.lastPinned/MVCContrib.Extras.release.zip"
    $releaseUrl = "http://build-oss.headspringlabs.com/guestAuth/repository/download/bt2/.lastPinned/MVCContrib.release.zip"
    $sourceUrl  = "http://build-oss.headspringlabs.com/guestAuth/repository/download/bt2/.lastPinned/MVCContrib.source.zip"

    $clnt = new-object System.Net.WebClient

    $clnt.DownloadFile($extrasUrl,"$($dir)MVCContrib.Extras.release.zip")
    $clnt.DownloadFile($releaseUrl, "$($dir)\MVCContrib.release.zip")
    $clnt.DownloadFile($sourceUrl, "$($dir)\MVCContrib.source.zip")
}

#DownloadFiles 
mkdir "release"
Extract-PAckage "$($dir)\MVCContrib.release.zip" "$($dir)\release"

[System.Diagnostics.FileVersionInfo]::GetVersionInfo("$($dir)\RELEASE\MvcContrib.dll").FileVersion

#& "..\bin\codeplex\createrelease.exe" "2.0.87.0" 
