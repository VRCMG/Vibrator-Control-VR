# Vibrator-Control-VR

Control Lovense toys via your VR controllers.
Default settings use the right trackpad on index controllers
You can select between multiple backends. 
You can also use the application to create a proxy connection to bypass the lovense api server. In this case the commands are send from the proxy server to the application running
It requires the Lovense Connect App (all Platforms) to be running in your network.

## HOW TO Proxy Mode Recommended (Person who wants to be controlled)

1. Start the Lovense Connect App on any Platform. Make sure its in the same network
1. Connect the toy/s with it
1. Start the Lovense Control VR executable on the PC
1. Enter 'h' to use the host mode and press enter
1. If you want to use the regular proxy server just press enter else enter a new hostname and press enter
1. It will search once for toys connected and lists them all with their accesscode used to control, their type and the host in your network hosting the toy.
The first person is now done. All other steps are for the second person

## HOW TO API Mode (Person who wants to be controlled)
1. Start the Lovense Connect App on Android or IOS.
1. Connect the toy/s with it
1. Visit https://lovense.er1807.de and scan the QR code with the app


## Person who controls

1. Open Steam VR first
1. Open the Lovense Control VR executable
1. Press enter to use the WS-API mode
1. Enter the hostname in case a specific one is used (same as in step 5) or leave it empty and press enter
1. Enter the accesscode provided by the other person for the toy that should be controlled
1. If you have index Controllers you should be done. If not you need to edit the bindings in the OVR Advanced Settings

## Controls (Index)
If you press B you can switch between Edge and Normal mode. MMore modes will follow

In normal mode you can use the right trackpad to control all vibrators. 

In Edge mode you control one motor with your right trackpad and one with your left trackpad.


## API Backend

The API Backend uses the Lovense API. The Server application is located under /Server and requires a developer token to use. 
The Server exposes under the root destination a page to generate new accesstokens and the QR code to register them for the Connect App

Every toy is currently assumed to be a Hush and only the Vibrate Command is used.

You can use the server under https://lovense.er1807.de to get an accesstoken.

Scan the QR Code with the Connect App and share the accesstoken with the person you want to control.

In the programm use the API Backend and enter lovense.er1807.de as the host and the accesstoken you got in the earlier step.

If you want to remove access again first remove the entry in the Connect app and afterwarts visit the remove link to remove the database entry.
Doing it the other way around might still allow it to registered again in the database.

This only works with the Android or IOS Lovense Connect App.

## Websocket API Backend
The Websocket API also allows connection via the Lovense Backend like above but also allows connections in the proxy mode. In API Mode commands are send to the Lovense API. In Proxy mode they are send to the other device running the app.

THe Websocket Backend is generally faster, since it uses a persistent connection and in proxy modes bypasses the Lovense API.



## Connect Backend

The Connect Backend uses the Connect App for mobile or PC directly.

The idea behind it is from the https://github.com/sextech/lovense

Since the Connectbacked normally only works locally a vpn Connection or something simelar is required.

This is not developed by or associated with Lovense or Hytto.
