# Lovense-Control-VR (unofficial) still WIP

This programm is still heavily WIP. It should work but...

Control Lovense toys via your VR controllers.
Default settings use the right trackpad on index controllers
You can select between multiple backends. 

# API Backend

The API Backend uses the Lovense API. The Server application is located under /Server and requires a developer token to use. 
The Server exposes under the root destination a page to generate new accesstokens and the QR code to register them for the Connect App

Every toy is currently assumed to be a Hush and only the Vibrate Command is used.

You can use the server under https://lovense.er1807.de to get an accesstoken.

Scan the QR Code with the Connect App and share the accesstoken with the person you want to control

In the programm use the API Backend and enter lovense.er1807.de as the host and the accesstoken you got in the earlier step

If you want to remove access again first remove the entry in the Connect app and afterwarts visit the remove link to remove the database entry.
Doing it the other way around might still allow it to registered again in the database.


# Connect Backend

The Connect Backend uses the Connect App for mobile or PC directly.

For the ConnectBackend I can't test currently but it should work. The idea behind it is from the https://github.com/sextech/lovense

Since the Connectbacked normally only works locally a vpn Connection or something simelar is required.

This is not developed by or associated with Lovense or Hytto.
