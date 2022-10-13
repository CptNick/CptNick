using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//class that stores the room information for later use such as where the rooms are and their dimensions
public class RoomInfo{
    public Vector3Int roomPos;
    public int roomWidth;
    public int roomHeight;
    public RoomInfo(Vector3Int position, int width, int height){
        roomPos = position; roomWidth = width; roomHeight = height;
    }
}

public class ProceduralGenerator : MonoBehaviour
{
    //defining the room size average (room will be width x length with the set bounds)
    private int roomWidth;
    private int roomHeight;
    private Random random = new Random();

    //min and max length of the sides of the room (all rooms will be rectangular)
    public int roomSizeMax = 17;
    public int roomSizeMin = 7;
    public int roomWallSize = 3;
    public int roomBufferScale = 10; 

    //min and max lengths of the corridoors (I'm setting the width to 2 for now so barrels can be added)
    public int corridoorMax = 200;
    public int corridoorMin = 100;
    private int corridoorLength;
    private int corridoorWidth = 2;
    private int corridoorWallHeight = 2;
    private int xMin, xMax, yMin, yMax;
    private string[] direction = {"right","left","up","down"};

    //Inputting the tilemaps (corridoor tilemap probably not needed)
    public Tilemap wall;
    public Tilemap room;
    public Tilemap barrels;
    public Tilemap door;

    //Inputting all the Tiles I will use
    //This is a bit clunky but I think it is the easiest way to access the correct assets
    public Tile basicFloor;
    public Tile crackedFloor;
    public Tile squareFloor;
    public Tile crackeSquareFloor;
    public Tile topWall;
    public Tile botWall;
    public Tile leftWall;
    public Tile rightWall;
    public Tile topLeftWall;
    public Tile botLeftWall;
    public Tile topRightWall;
    public Tile botRightWall;
    public Tile insTopLeftWall;
    public Tile insBotLeftWall;
    public Tile insTopRightWall;
    public Tile insBotRightWall;

    public Tile basicWall;
    public Tile smallVine;
    public Tile bigVine1;
    public Tile bigVine2;

    public Tile[] closedDoor;
    public Tile[] openDoor;
    
    public Tile barrel;
    public Tile health;

    public List<RoomInfo> roomInfoArray = new List<RoomInfo>();

    private Vector2Int structure = new Vector2Int(0,0);
    private List<Vector2Int> structureArray = new List<Vector2Int>();

    [Range(1,100)]
    public int texturePercent = 20;
    public Tile[] textureTiles;

    [Range(1,100)]
    public int barrelPercent = 2;
    public Vector3Int doorPos;
    private Vector2Int doorRoom;

    public void ClearLevel(){
        wall.ClearAllTiles();
        room.ClearAllTiles();
        barrels.ClearAllTiles();
        door.ClearAllTiles();
        roomInfoArray.Clear();
        structureArray.Clear();
    }

    //make a room with roomPosition being the bottom left corner of the room (returns the width and length of the room)
    void MakeRoom(Vector3Int roomPosition, int roomWidth, int roomHeight){

        xMin = 1;
        xMax = roomWidth;
        yMin = 1;
        yMax = roomHeight;

        //This makes the basic floors
        for(int x = xMin; x <= xMax; x++){
            for(int y = yMin; y <= yMax; y++){

                //introduce random floor texturing
                int rand1 = Random.Range(1,100);
                Vector3Int floorPos = new Vector3Int(roomPosition.x + x, roomPosition.y + y, 0);
                if(rand1 <= texturePercent){
                    int rand2 = Random.Range(0,textureTiles.Length);
                    room.SetTile(floorPos, textureTiles[rand2]);
                }
                else{
                    room.SetTile(floorPos, basicFloor);
                }
            }
        }

        //adding the wall size to the top and 1 for the perimeter
        xMin -= 1;
        xMax += 1;
        yMin -= 1;
        yMax += roomWallSize + 1;

        //This makes the walls surrounding the floors (+3 for room wall height)
        for(int x = xMin; x <= xMax; x++){
            for(int y = yMin; y <= yMax; y++){
                Vector3Int wallPos = new Vector3Int(roomPosition.x + x, roomPosition.y + y, 0);
                //bottom Left
                if(x == xMin && y == yMin){
                    wall.SetTile(wallPos, botLeftWall);
                }
                //bottom Right
                if(x == xMax && y == yMin){
                    wall.SetTile(wallPos, botRightWall);
                }
                //top Left
                if(x == xMin && y == yMax){
                    wall.SetTile(wallPos, topLeftWall);
                }
                //top Right
                if(x == xMax && y == yMax){
                    wall.SetTile(wallPos, topRightWall);
                }
                //Left
                if(x == xMin && y > yMin && y < yMax){
                    wall.SetTile(wallPos, leftWall);
                }
                //Right
                if(x == xMax && y > yMin && y < yMax){
                    wall.SetTile(wallPos, rightWall);
                }
                //Bottom
                if(x > xMin && x < xMax && y == yMin){
                    wall.SetTile(wallPos, botWall);
                }
                //Top wall
                if(x > xMin && x < xMax && y > roomHeight && y < yMax){
                    wall.SetTile(wallPos, basicWall);
                }
                //Top++
                if(x > xMin && x < xMax &&  y == yMax){
                    wall.SetTile(wallPos, topWall);
                }
            }
        }

        //1 vine pattern for a small room
        if(roomWidth < 10){
            int pattern = Random.Range(0,5);
            int randPos = Random.Range(-1,2);
            Vector3Int pos = new Vector3Int(roomPosition.x + roomWidth/2 + randPos, roomPosition.y + yMax - 1, 0);
            WallVinePatters(pos, pattern);
        }
        //2 patterns for a larger room
        if(roomWidth >= 10){
            int pattern1 = Random.Range(0,5);
            int randPos1 = Random.Range(-2,-1);
            Vector3Int pos1 = new Vector3Int(roomPosition.x + roomWidth/2 + randPos1, roomPosition.y + yMax - 1, 0);
            WallVinePatters(pos1, pattern1);
            int pattern2 = Random.Range(0,5);
            int randPos2 = Random.Range(2,5);
            Vector3Int pos2 = new Vector3Int(roomPosition.x + roomWidth/2 + randPos2, roomPosition.y + yMax - 1, 0);
            WallVinePatters(pos2, pattern2);
        }
    }

    void MakeCorridoor(Vector3Int corridoorPosition, string direction, int corridoorLength){

        //corridoorPosition will be the edge of the room floor
        if(direction == "right"){
            xMin = 0;
            xMax = corridoorLength;
            yMin = 0;
            yMax = corridoorWidth;
        }
        if(direction == "left"){
            xMin = -corridoorLength;
            xMax = 0;
            yMin = 0;
            yMax = corridoorWidth;
        }
        if(direction == "up"){
            xMin = 0;
            xMax = corridoorWidth - 1;
            yMin = 0;
            yMax = corridoorLength;
        }
        if(direction == "down"){
            xMin = 0;
            xMax = corridoorWidth - 1;
            yMin = -corridoorLength;
            yMax = 0;
        }

        for(int x = xMin; x <= xMax; x++){
            for(int y = yMin; y <= yMax; y++){
                int rand = Random.Range(1,100);
                Vector3Int floorPos = new Vector3Int(corridoorPosition.x + x, corridoorPosition.y + y, 0);
                if(rand <= texturePercent){
                    int rand2 = Random.Range(0,textureTiles.Length);
                    room.SetTile(floorPos, textureTiles[rand2]);
                }
                else{
                    room.SetTile(floorPos, basicFloor);
                }
            }
        }

        //for the left and right corridoors
        if(direction == "right" || direction == "left"){
            //for the left and right corridoors the wall extends 1 below and 3 above the floor tiles where the extra 1 is the caps to the walls
            int floorPosMin = yMin;
            int floorPosMax = yMax;
            yMin -= 1;
            yMax += corridoorWallHeight;

            for(int x = xMin; x <= xMax; x++){
                for(int y = yMin; y <= yMax; y++){

                    Vector3Int wallPos = new Vector3Int(corridoorPosition.x + x, corridoorPosition.y + y, 0);
                    //just clearing any stray walls
                    wall.SetTile(wallPos, null);

                    //Bottom left
                    if(x == xMin && y == yMin){
                        wall.SetTile(wallPos, insBotLeftWall);
                    }
                    //Bottom right
                    if(x == xMax && y == yMin){
                        wall.SetTile(wallPos, insBotRightWall);
                    }
                    //Top left
                    if(x == xMin && y == yMax){
                        wall.SetTile(wallPos, insTopLeftWall);
                    }
                    //Top right
                    if(x == xMax && y == yMax){
                        wall.SetTile(wallPos, insTopRightWall);
                    }
                    //Top
                    if(x > xMin && x < xMax && y == yMax){
                        wall.SetTile(wallPos, topWall);
                    }
                    //Bottom
                    if(x > xMin && x < xMax && y == yMin){
                        wall.SetTile(wallPos, botWall);
                    }
                    //Basic Wall
                    if(y >= floorPosMax && y < yMax){
                        wall.SetTile(wallPos, basicWall);
                    }
                }
            }
        }

        //for the top and bottom corridoors
        if(direction == "up" || direction == "down"){

            int floorPosMin = yMin;
            int floorPosMax = yMax;

            xMin -= 1;
            xMax += 1;

            for(int x = xMin; x <= xMax; x++){
                for(int y = yMin; y <= yMax; y++){

                    Vector3Int wallPos = new Vector3Int(corridoorPosition.x + x, corridoorPosition.y + y, 0);
                    //just clearing any stray walls
                    wall.SetTile(wallPos, null);

                    //Bottom left (ins wall)
                    if(x == xMin && y == yMin + corridoorWallHeight + 1){
                        wall.SetTile(wallPos, insTopRightWall);
                    }
                    //Left
                    if(x == xMin && y > yMin + corridoorWallHeight + 1 && y < yMax){
                        wall.SetTile(wallPos, leftWall);
                    }
                    //Right
                    if(x == xMax && y > yMin + corridoorWallHeight + 1 && y < yMax){
                        wall.SetTile(wallPos, rightWall);
                    }
                    //Bottom right (ins wall)
                    if(x == xMax && y == yMin + corridoorWallHeight + 1){
                        wall.SetTile(wallPos, insTopLeftWall);
                    }
                    //Top left (uses ins bottom left sprite)
                    if(x == xMin && y == yMax){
                        wall.SetTile(wallPos, insBotRightWall);
                    }
                    //Top right ("")
                    if(x == xMax && y == yMax){
                        wall.SetTile(wallPos, insBotLeftWall);
                    }
                    //Basic Wall
                    if(y >= yMin && y <= yMin + corridoorWallHeight && (x == xMin || x == xMax)){
                        wall.SetTile(wallPos, basicWall);
                    }
                }
            }
        }
    }
    
    void MakeClosedDoor(){
        
        //find a single room that has no up corridoor
        //we will check each room to see if it has no up corridoor and the latest room that meets this condition 
        //will have the end corridoor (should be far away from the start)
        bool canBeEndRoom = true;
        foreach(Vector2Int vec1 in structureArray){
            foreach(Vector2Int vec2 in structureArray){
                //room above
                if(vec1.y + 1 == vec2.y && vec1.x == vec2.x){
                    canBeEndRoom = false;
                }
            }
            
            //if room has no room above then store the room information
            if(canBeEndRoom){
                roomHeight = roomInfoArray[structureArray.IndexOf(vec1)].roomHeight;
                doorPos = roomInfoArray[structureArray.IndexOf(vec1)].roomPos;
                doorPos.x += roomWidth/2 - 1;
                doorPos.y += roomHeight + 1;
                doorRoom = vec1;
            }
            canBeEndRoom = true;
        }

        //this count feature is just to generalise to any sized door (really only effects the horizontal when closedDoor.Length is divided by 2)
        //dived by door height for general case
        //painted in from bottom to top
        int count = 0;
        Vector3Int tempPos = doorPos;
        foreach(Tile doorTile in closedDoor){
            count += 1;
            wall.SetTile(tempPos, null);
            door.SetTile(tempPos, doorTile);
            if(count < closedDoor.Length/2){
                tempPos.x += 1;
            }
            else{
                tempPos.y += 1;
                tempPos.x -= count - 1;
                count = 0;
            }
        }
    }

    public void OpenDoor(){
        int count = 0;
        Vector3Int tempPos = doorPos;
        foreach(Tile doorTile in openDoor){
            count += 1;
            wall.SetTile(tempPos, null);
            door.SetTile(tempPos, doorTile);
            if(count < openDoor.Length/2){
                tempPos.x += 1;
            }
            else{
                tempPos.y += 1;
                tempPos.x -= count - 1;
                count = 0;
            }
        }
    }

    //method makes vine patters with given position of top of wall
    void WallVinePatters(Vector3Int pos, int pattern){
        //random doesnt seem to actually include max value despite what unity says
        if(pattern == 0){
            wall.SetTile(pos, smallVine);
        }
        if(pattern == 1){
            wall.SetTile(pos, bigVine1);
            pos.y -= 1;
            wall.SetTile(pos, smallVine);
        }
        if(pattern == 2){
            wall.SetTile(pos, bigVine1);
            pos.x += 1;
            wall.SetTile(pos, smallVine);
            pos.y -= 1;
            pos.x -= 1;
            wall.SetTile(pos, bigVine1);
        }
        if(pattern == 3){
            wall.SetTile(pos, bigVine1);
            pos.y -= 1;
            wall.SetTile(pos, bigVine2);
            pos.y -= 1;
            wall.SetTile(pos, smallVine);
        }
        if(pattern == 4){
            wall.SetTile(pos, bigVine1);
            pos.x -= 1;
            wall.SetTile(pos, bigVine2);
            pos.y -= 1;
            pos.x += 1;
            wall.SetTile(pos, bigVine1);
            pos.y -= 1;
            wall.SetTile(pos, bigVine2);
        }
    }

    void MakeBarrels(Vector3Int roomPosition, int width, int height, int index){
        //place some barrels arround the circumference for natural looks
        //choose random corner to have 3 barrels
        int rand3 = Random.Range(0, 4);
        Vector3Int barrelPos = new Vector3Int(0, 0, 0);

        //top left
        if (rand3 == 0)
        {
            barrelPos.x = roomPosition.x + 1;
            barrelPos.y = roomPosition.y + height;
            barrels.SetTile(barrelPos, barrel);
            barrelPos.x += 1;
            barrels.SetTile(barrelPos, barrel);
            barrelPos.x -= 1;
            barrelPos.y -= 1;
            barrels.SetTile(barrelPos, barrel);
            
        }
        //top right
        if (rand3 == 1)
        {
            barrelPos.x = roomPosition.x + width;
            barrelPos.y = roomPosition.y + height;
            barrels.SetTile(barrelPos, barrel);
            barrelPos.x -= 1;
            barrels.SetTile(barrelPos, barrel);
            barrelPos.x += 1;
            barrelPos.y -= 1;
            barrels.SetTile(barrelPos, barrel);
        }
        //bot left
        if (rand3 == 2)
        {
            barrelPos.x = roomPosition.x + 1;
            barrelPos.y = roomPosition.y + 1;
            barrels.SetTile(barrelPos, barrel);
            barrelPos.x += 1;
            barrels.SetTile(barrelPos, barrel);
            barrelPos.x -= 1;
            barrelPos.y += 1;
            barrels.SetTile(barrelPos, barrel);
        }
        //bot right
        if (rand3 == 3)
        {
            barrelPos.x = roomPosition.x + width;
            barrelPos.y = roomPosition.y + 1;
            barrels.SetTile(barrelPos, barrel);
            barrelPos.x -= 1;
            barrels.SetTile(barrelPos, barrel);
            barrelPos.x += 1;
            barrelPos.y += 1;
            barrels.SetTile(barrelPos, barrel);
        }

        // Im pretty sure there is a way of putting all these loops together, but since this game is small it doesnt really matter

        List<Vector2Int> tempList = structureArray;
        //find out if a wall has no corridoor

        Vector2Int vec1 = tempList[index]; 
        bool freeRight = true;
        bool freeLeft = true;
        bool freeTop = true;
        bool freeBot = true;
        foreach (Vector2Int vec2 in tempList)
        {
            //room on right
            if (vec1.x + 1 == vec2.x && vec1.y == vec2.y)
            {
                freeRight = false;
            }
            //room on left
            if (vec1.x - 1 == vec2.x && vec1.y == vec2.y)
            {
                freeLeft = false;
            }
            //room above
            if (vec1.y + 1 == vec2.y && vec1.x == vec2.x)
            {
                freeTop = false;
            }
            //room below
            if (vec1.y - 1 == vec2.y && vec1.x == vec2.x)
            {
                freeBot = false;
            }
        }
        //place 2 barrels somewhere flush with the free walls
        if (freeRight)
        {
            barrelPos.x = roomPosition.x + width;
            barrelPos.y = roomPosition.y + Random.Range(2, height - 3);
            barrels.SetTile(barrelPos, barrel);
            barrelPos.y += 1;
            barrels.SetTile(barrelPos, barrel);
        }
        if (freeLeft)
        {
            barrelPos.x = roomPosition.x + 1;
            barrelPos.y = roomPosition.y + Random.Range(2, height - 3);
            barrels.SetTile(barrelPos, barrel);
            barrelPos.y += 1;
            barrels.SetTile(barrelPos, barrel);
        }

        //making sure the door isnt blocked (you could still go through but it looks bad)
        if (freeTop && vec1 != doorRoom)
        {
            barrelPos.x = roomPosition.x + Random.Range(2, width - 3);
            barrelPos.y = roomPosition.y + height;
            barrels.SetTile(barrelPos, barrel);
            barrelPos.x += 1;
            barrels.SetTile(barrelPos, barrel);
        }
        if (freeBot)
        {
            barrelPos.x = roomPosition.x + Random.Range(2, width - 3);
            barrelPos.y = roomPosition.y + 1;
            barrels.SetTile(barrelPos, barrel);
            barrelPos.x += 1;
            barrels.SetTile(barrelPos, barrel);
        }

        //for large rooms can place more central barrels for more cover in the bigger space
        //can measure this with the room area
        int area = width*height;
        int cutoff = 100;

        //large room
        if(area > cutoff){
            //place towards the left centre of the room with a gap of at least 2 between the barrels and the wall
            barrelPos.x = roomPosition.x + Random.Range(2, width/2 - 2);
            barrelPos.y = roomPosition.y + Random.Range(2, height - 2);
            barrels.SetTile(barrelPos, barrel);

            //chance to add an extra barrel next to the place barrel
            barrelPos.x += Random.Range(0,2);
            barrelPos.y += Random.Range(0,2);
            barrels.SetTile(barrelPos, barrel);

            barrelPos.x = roomPosition.x + Random.Range(width - 2, width/2 + 2);
            barrelPos.y = roomPosition.y + Random.Range(2, height - 2);
            barrels.SetTile(barrelPos, barrel);

            //chance to add an extra barrel next to the place barrel (can be 0 or 1 in each direction)
            barrelPos.x += Random.Range(0,2);
            barrelPos.y += Random.Range(0,2);
            barrels.SetTile(barrelPos, barrel);
        }
        else{
            barrelPos.x = roomPosition.x + Random.Range(2, width - 2);
            barrelPos.y = roomPosition.y + Random.Range(2, height - 2);
            barrels.SetTile(barrelPos, barrel);

            //chance to add an extra barrel next to the place barrel
            barrelPos.x += Random.Range(0,2);
            barrelPos.y += Random.Range(0,2);
            barrels.SetTile(barrelPos, barrel);
        }
    }

    public void Generate(int numRooms){

        for(int i = 1; i <= numRooms; i++){

            //create new room each loop at a place holder location (used to create the instance of the roomInfo class)
            Vector3Int placeHolderPos = new Vector3Int(0,0,0);
            roomWidth = Random.Range(roomSizeMin, roomSizeMax);
            roomHeight = Random.Range(roomSizeMin, roomSizeMax);


            //store this info in an instance of the RoomInfo class
            RoomInfo roomInfo = new RoomInfo(placeHolderPos, roomWidth, roomHeight);

            //add each instance to an array which we can then call from anywhere
            roomInfoArray.Add(roomInfo);
        }

        structureArray.Add(structure);
        //structureArray.SetValue(structure, 0);
        
        //use i < numRooms here since we are add an extra room at (0,0) before the loop 
        for(int i = 1; i < numRooms; i++){

            //not sure about how random this funciton really is since it technically takes floats not ints
            int index = Random.Range(0, 4);

            //choose a direction to move from current room position
            if(direction[index] == "right"){
                structure.x  += 1;
            }
            if(direction[index] == "left"){
                structure.x  -= 1;
            }
            if(direction[index] == "up"){
                structure.y  += 1;
            }
            if(direction[index] == "down"){
                structure.y  -= 1;
            }

            bool overlap = false;
            //check each room position to see if we will overlap
            foreach(Vector2Int pos in structureArray){
                //if structure = pos it means that the is already a room in the position we are trying to place a room
                if(pos == structure){
                    overlap = true;
                }
            }

            //if we find an overlap then we loop through the same itteration again (with the new position i.e. in the overlap) to get a new, free position
            if(overlap){
                i -= 1;
            }
            //if no overlap we add the new vector to the structure array
            if(!overlap){
                structureArray.Add(structure);
            }

        }

        //here we take the room positions from the structure array and convert them into the tilemap coordinates to be placed
        //use i = 0 to i < numRooms since the index here matters
        for(int i = 0; i < numRooms; i++){
            //place centre of room at structureArray position
            roomInfoArray[i].roomPos.x = structureArray[i].x*roomBufferScale -  roomInfoArray[i].roomWidth/2;
            roomInfoArray[i].roomPos.y = structureArray[i].y*roomBufferScale - roomWallSize - roomInfoArray[i].roomHeight/2;
        }
        

        //create the rooms
        for(int i = 0; i < numRooms; i++){
            MakeRoom(roomInfoArray[i].roomPos, roomInfoArray[i].roomWidth, roomInfoArray[i].roomHeight);
        }
        
        //made this tempList because I was going to remove things from it but changed this in the end
        //I think I can just use structureArray since I'm not editing it anymore
        List<Vector2Int> tempList = structureArray;

        //find which rooms are adjacent to create the correct corridoors
        foreach(Vector2Int vec1 in tempList){
            foreach(Vector2Int vec2 in tempList){

                //room on left and right
                if(vec1.x + 1 == vec2.x && vec1.y == vec2.y){
                    roomWidth = roomInfoArray[tempList.IndexOf(vec1)].roomWidth;
                    corridoorLength = roomInfoArray[tempList.IndexOf(vec2)].roomPos.x - roomInfoArray[tempList.IndexOf(vec1)].roomPos.x - roomWidth - 1;
                    Vector3Int pos = roomInfoArray[tempList.IndexOf(vec1)].roomPos;

                    int furtherUp = roomInfoArray[tempList.IndexOf(vec2)].roomPos.y + roomInfoArray[tempList.IndexOf(vec2)].roomHeight - roomInfoArray[tempList.IndexOf(vec1)].roomPos.y - roomInfoArray[tempList.IndexOf(vec1)].roomHeight;
                    int furtherDown = roomInfoArray[tempList.IndexOf(vec2)].roomPos.y - roomInfoArray[tempList.IndexOf(vec1)].roomPos.y;

                    int maxUp = roomInfoArray[tempList.IndexOf(vec2)].roomPos.y + roomInfoArray[tempList.IndexOf(vec2)].roomHeight - corridoorWidth - 2;
                    int maxDown = roomInfoArray[tempList.IndexOf(vec2)].roomPos.y + 4;

                    if(furtherUp > 0 ){
                        maxUp = roomInfoArray[tempList.IndexOf(vec1)].roomPos.y + roomInfoArray[tempList.IndexOf(vec1)].roomHeight - corridoorWidth - 2;
                    }
                    if(furtherDown < 0){
                        maxDown = roomInfoArray[tempList.IndexOf(vec1)].roomPos.y + 4;
                    }

                    //choose random y coordinate between max values to make it feel more natural
                    int corridoorPosY = Random.Range(maxDown, maxUp);

                    pos.x += roomWidth + 1;
                    pos.y = corridoorPosY;
                    MakeCorridoor(pos, "right", corridoorLength);
                }

                //room above and below
                if(vec1.y + 1 == vec2.y && vec1.x == vec2.x){
                    roomHeight = roomInfoArray[tempList.IndexOf(vec1)].roomHeight;
                    corridoorLength = roomInfoArray[tempList.IndexOf(vec2)].roomPos.y - roomInfoArray[tempList.IndexOf(vec1)].roomPos.y - roomHeight - 1;
                    Vector3Int pos = roomInfoArray[tempList.IndexOf(vec1)].roomPos;

                    //check to see if the room above is furhter left than the current room
                    int furtherLeft = roomInfoArray[tempList.IndexOf(vec2)].roomPos.x - roomInfoArray[tempList.IndexOf(vec1)].roomPos.x;
                    int furtherRight = roomInfoArray[tempList.IndexOf(vec2)].roomPos.x + roomInfoArray[tempList.IndexOf(vec2)].roomWidth - roomInfoArray[tempList.IndexOf(vec1)].roomPos.x - roomInfoArray[tempList.IndexOf(vec1)].roomWidth;

                    //if further left is negative it means that the room above is shifted left compared to the room in vec1
                    //if further right is positive it means that the room above is shifted right compared to the room in vec1

                    int maxRight = roomInfoArray[tempList.IndexOf(vec2)].roomPos.x + roomInfoArray[tempList.IndexOf(vec2)].roomWidth - corridoorWidth - 2;
                    int maxLeft = roomInfoArray[tempList.IndexOf(vec2)].roomPos.x + 4;

                    if(furtherRight > 0 ){
                        maxRight = roomInfoArray[tempList.IndexOf(vec1)].roomPos.x + roomInfoArray[tempList.IndexOf(vec1)].roomWidth - corridoorWidth - 2;
                    }
                    if(furtherLeft < 0){
                        maxLeft = roomInfoArray[tempList.IndexOf(vec1)].roomPos.x + 4;
                    }

                    int corridoorPosX = Random.Range(maxLeft, maxRight);

                    pos.x = corridoorPosX;
                    pos.y += roomHeight + 1;
                    MakeCorridoor(pos, "up", corridoorLength);
                }
            }
        }
        MakeClosedDoor();
        //add the barrels
        for(int i = 0; i < roomInfoArray.Count;i++){
            MakeBarrels(roomInfoArray[i].roomPos, roomInfoArray[i].roomWidth, roomInfoArray[i].roomHeight, i);
        }
    }
}

