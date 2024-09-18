THANK YOU FOR PURCHASE!!

<<THE ASSET NEED PHOTON NETWORK PUN 2>>
<<DOWNLOAD PHOTON NETWORK PUN2 IN ASSET STORE>>
https://assetstore.unity.com/packages/tools/network/pun-2-free-119922


/!\ ALL LINES IS COMMENTED /!\


SCRIPT "GameManager.cs":
	1/ IN "GameManager" COMPONENT, WHEN YOU ARE CONNECTED LINE 32, TO PHOTON, LAUNCH COMMAND "OnConnectedToMaster()" LINE 36, AND IF YOU IS MASTER (host variable is true line 29), YOU 
	   CREATE ROOM, IF YOU IS NOT MASTER (host variable is false), YOU JOIN THE RANDOM ROOM. IF JOIN RANDOM ROOM IS FAILED, LAUNCH COMMAND "OnPhotonRandomJoinFailed()"
	   LINE 53 AND AUTOMATICALLY YOU CREATE THE NEW ROOM LINE 58.

	2/ LINE 41, THE COMMAND "OnJoinedRoom()" IS CALLED WHEN YOU ENTER IN ROOM AND YOU ADD IN LIST PLAYER IN COMPONENT INSTANCE "MultiplayerManager.cs" WHITE 
	   COMMAND "launch()" IN "MultiplayerManager.cs".

	3/ LINE 49, WHEN YOU CONNECTED IN ROOM AND IF THE VARIABLE "TeamScene" IS "TRUE" SO ACTIVE "CHOOSETEAM" GAMEOBJECT FOR CHOOSING TEAM.
	   IF THE VARIABLE "TeamScene" IS "TRUE", SO THE SCOREBOARD PANEL IS TEAMDEATMATCH OR IF THE VARIABLE "TeamScene" IS "FALSE", THE SCOREBOARD PANEL IS
	   DEATMATCH.



SCRIPT "UIGlobal.cs"
		It is to manage all UI in game.




SCRIPT "PlayerNumber.cs"
		Assign numbers to Players in a room. Uses Room custom Properties
		This script belongs to "Photon". Look in your "Asset/Photon/PhotonUnityNetworking/UtilityScripts/PhotonPlayer/".




FOR HELP:
MAIL = paradox.ultimate@hotmail.fr
WEBSITE = http://www.ricl-chatland.fr