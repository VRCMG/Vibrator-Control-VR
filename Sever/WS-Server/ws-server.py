import asyncio, json, websockets, pymongo, uuid, requests, os, sys

mongo = pymongo.MongoClient("mongodb://mongo:27017/")
toys = mongo["lovense"]["toys"]

websocket_toys = {}

token = os.environ.get('token')

token = "OcTQR+F8LZj+WP66RvTomnX7bgRluAUSUcv/wbzg8WU69ixdSiyRk/raMVD8vYSL"

if token is None:
    print("No token provided")
    sys.exit(-1)

def getEntryfromDB(accesscode):
    global toys

    query = { "$or": [{"_id": accesscode}, {"short": accesscode}] }

    enties = toys.find(query)

    for entry in enties:
        return entry
    else:
        return None


def deleteAccesscodeFromDB(accesscode):
    global toys
    query = { "$or": [{"_id": accesscode}, {"short": accesscode}] }
    toys.delete_one(query)

def insertToy(uid, utoken, toy, short):
    global toys
    entry = { "uid": uid, "utoken": utoken, "toy":[toy], "_id":uid, "short": short, "type": "WS" }
    toys.insert_one(entry)

async def sendCommand(jsonobj):
    global toys, websocket_toys, token
    uid_rec = jsonobj['uid']
    entry = getEntryfromDB(uid_rec)

    if(entry is None):
        return 'Invalid code'
        
    accesscode = entry['uid']
    toy = entry['toy'][0]
    if(entry['type'] == "LC"):
        accesscode = entry['uid']
        toy = entry['toy'][0]
        url = f"https://api.lovense.com/api/lan/command?token={token}&uid={accesscode}&t={toy}&v={jsonobj['value']}&command={jsonobj['action']}"
        requests.request("POST", url)
    elif entry['type'] == "WS":
        if(uid_rec not in websocket_toys):
            return 'Toy not WS List found'
        await websocket_toys[uid_rec][1].send(json.dumps({"type": "command", "command": jsonobj['action'], "toy": toy, "value": jsonobj['value']}))
    return "Success"


async def handler(websocket, path):
    global websocket_toys
    local_toys = []
    while True:
        message = await websocket.recv()
        jsonobj = json.loads(message)

        messagetype = jsonobj["type"]

        if (messagetype=="register-toy"):
            uid = uuid.uuid4().hex
            short = uid[:6]
            insertToy(uid, 'ND', jsonobj["toy"], short)
            websocket_toys[short] = (jsonobj["toy"], websocket)
            local_toys.append(short)
            await websocket.send(json.dumps({"type": "register-toy-result", "uid": short, "message": "inserted", "custom": jsonobj["custom"]}))
        elif (messagetype=="remove-toy"):
            uid = jsonobj["uid"]
            if uid in local_toys:
                deleteAccesscodeFromDB(uid)
                if uid in websocket_toys:
                    del websocket_toys[uid]
                local_toys.remove(uid)
                await websocket.send(json.dumps({"type": "remove-toy-result","message": "deleted"}))
            else:
                await websocket.send(json.dumps({"type": "remove-toy-result","message": "not your toy"}))
        elif (messagetype=="send-command"):
            await websocket.send(json.dumps({"type": "send-command-result","message": await sendCommand(jsonobj)}))


start_server = websockets.serve(handler, "0.0.0.0", 8765)

asyncio.get_event_loop().run_until_complete(start_server)
asyncio.get_event_loop().run_forever()