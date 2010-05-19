function DownloadFiles {
    remove-item "MVCContrib.Extras.release.zip"
    remove-item "MVCContrib.release.zip"
    remove-item "MVCContrib.source.zip"
    $extrasUrl  = "http://build1.headspringlabs.com/guestAuth/repository/download/bt2/.lastPinned/MVCContrib.Extras.release.zip"
    $releaseUrl = "http://build1.headspringlabs.com/guestAuth/repository/download/bt2/.lastPinned/MVCContrib.release.zip"
    $sourceUrl  = "http://build1.headspringlabs.com/guestAuth/repository/download/bt2/.lastPinned/MVCContrib.source.zip"

    $clnt = new-object System.Net.WebClient

    $clnt.DownloadFile($extrasUrl,"MVCContrib.Extras.release.zip")
    $clnt.DownloadFile($releaseUrl,"MVCContrib.release.zip")
    $clnt.DownloadFile($sourceUrl,"MVCContrib.source.zip")
}

DownloadFiles 
& "..\bin\codeplex\createrelease.exe" "2.0.35.0"
