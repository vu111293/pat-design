using System;
using System.Text;

namespace PAT.Common.Examples
{
    public class Examples
    {
        public static string LoadLiftSystem(int floorSize, int lift, int user)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("#import \"PAT.Lib.Example\";");
            sb.AppendLine();

            sb.AppendLine("//A single lift system with NoOfFloors floors;");
            sb.AppendLine("#define NoOfFloors " + floorSize + ";");
            sb.AppendLine("#define NoOfLifts " + lift + ";");
            sb.AppendLine("#define NoOfUsers " + user + ";");
            sb.AppendLine();
            sb.AppendLine("//this array models the external requests; ");
            sb.AppendLine("//extrequests[i] = 1 denotes there is an upward request at i-floor; 2 for downward; 3 for both direction; 0 for no request;");
            sb.AppendLine("var extrequestsUP[NoOfFloors]; ");
            sb.AppendLine("var extrequestsDOWN[NoOfFloors]; ");
            sb.AppendLine("//this array models the internal requests; ");
            sb.AppendLine("//intrequests[i][j] = 1 if there is a request for i-floor for lift-j; 0 for no request;");
            sb.AppendLine("var intrequests[NoOfLifts][NoOfFloors]; ");
            sb.AppendLine();
            sb.AppendLine("//the level of the lift door opens at. -1 mean the door is closed.");
            sb.AppendLine("var door = [-1(NoOfLifts)]; //initiate an array of -1 with length NoOfLifts");
            //sb.AppendLine();
            //sb.AppendLine("//system variables;");
            //sb.AppendLine("var CheckResult[NoOfLifts]; ");
            sb.AppendLine();
            sb.AppendLine("//a lift system consists of multiple users and lifts running in parallel");
            sb.AppendLine("LiftSystem() = (||| {NoOfUsers} @ User())");
            sb.AppendLine("				|||	");
            sb.AppendLine("			   (||| x:{0..NoOfLifts-1} @ Lift(x, 0, 1)); ");
            sb.AppendLine();
            sb.AppendLine("//the following models the behaviors of the users;");
            sb.AppendLine("User() = []pos:{0..NoOfFloors-1}@ (ExternalPush(pos); UserWaiting(pos));");
            sb.AppendLine();
            sb.AppendLine("//the following models the behaviors of the user pushing external buttons;");
            sb.AppendLine("ExternalPush(pos) = case {");
            sb.AppendLine("			pos == 0 : pushup.pos{extrequestsUP[pos] = 1;} -> Skip");
            sb.AppendLine("			pos == NoOfFloors-1 : pushdown.pos{extrequestsDOWN[pos] = 1;} -> Skip");
            sb.AppendLine("			default : pushup.pos{extrequestsUP[pos] = 1;} -> Skip ");
            sb.AppendLine("				[] pushdown.pos{extrequestsDOWN[pos] = 1;} -> Skip");
            sb.AppendLine("		 };");
            sb.AppendLine();
            sb.AppendLine("//the following models the behaviors of the user waiting and entering the lift;");
            sb.AppendLine("UserWaiting(pos) = [] i:{0..NoOfLifts-1} @ ");
            sb.AppendLine("			([door[i] == pos]enter.i -> ([]y:{0..NoOfFloors-1}@(push.y{intrequests[i][y] = 1;} -> ");
            sb.AppendLine("			([door[i] == y]exit.i -> User()))));");
            sb.AppendLine();
            sb.AppendLine("//the following models the behaviors of the lift; Initially, the lift is at 0-th floor ready to go upwards.");
            sb.AppendLine("Lift(i, level, direction) = ");
            sb.AppendLine("		//if there are requests for the current floor, open the door. ");
            sb.AppendLine("		ifa (intrequests[i][level] != 0 || (direction == 1 && extrequestsUP[level] == 1) || (direction == -1 && extrequestsDOWN[level] == 1)) {");
            sb.AppendLine("			opendoor.i.level{door[i] = level; intrequests[i][level] = 0;");
            sb.AppendLine("				if (direction > 0) {");
            sb.AppendLine("					extrequestsUP[level] =0;");
            sb.AppendLine("				} else {");
            sb.AppendLine("					extrequestsDOWN[level] = 0;");
            sb.AppendLine("				}");
            sb.AppendLine("			} -> close.i.level{door[i] = -1;} -> Lift(i, level, direction)");
            sb.AppendLine("		}");
            sb.AppendLine("		else {");
            sb.AppendLine("			//to check whether there are request on the travelling direction.");
            sb.AppendLine("			checkIfToMove.i.level ->");
            //sb.AppendLine("             var Counter = 0;");
            //sb.AppendLine("             Counter = level+direction; CheckResult[i] = 0;");
            //sb.AppendLine("				while (Counter >= 0 && Counter < NoOfFloors && CheckResult[i] == 0) {");
            //sb.AppendLine("					if (extrequestsUP[Counter] != 0 || extrequestsDOWN[Counter] != 0 || intrequests[i][Counter] != 0) {");
            //sb.AppendLine("						CheckResult[i] = 1;");
            //sb.AppendLine("					}");
            //sb.AppendLine("					else {");
            //sb.AppendLine("						Counter=Counter+direction;");
            //sb.AppendLine("					}");
            //sb.AppendLine("				}");
            //sb.AppendLine("			} ->");
            sb.AppendLine("			ifa (call(CheckIfToMove, level, direction, i, NoOfFloors, intrequests, extrequestsUP, extrequestsDOWN)) {");
            sb.AppendLine("				moving.i.level.direction -> ");
            sb.AppendLine("				ifa (level+direction == 0 || level+direction == NoOfFloors-1) {");
            sb.AppendLine("					Lift(i, level+direction, -1*direction)");
            sb.AppendLine("				}");
            sb.AppendLine("				else {");
            sb.AppendLine("					Lift(i, level+direction, direction)");
            sb.AppendLine("				}");
            sb.AppendLine("			}");
            sb.AppendLine("			else {");
            sb.AppendLine("				ifa ((level == 0 && direction == 1) || (level == NoOfFloors-1 && direction == -1)) {");
            sb.AppendLine("					Lift(i, level, direction)");
            sb.AppendLine("				}");
            sb.AppendLine("				else {");
            sb.AppendLine("					changedir.i.level -> Lift(i, level, -1*direction)");
            sb.AppendLine("				}");
            sb.AppendLine("			}");
            sb.AppendLine("		};");
            sb.AppendLine();
            sb.AppendLine("#assert LiftSystem() deadlockfree;");
            sb.AppendLine();
            sb.AppendLine("//if there is an external request at first floor, it will eventually be served. ");
            sb.AppendLine("#define on extrequestsUP[0] == 1;");
            sb.AppendLine("#define off extrequestsUP[0] == 0;");
            sb.AppendLine("#define on2 intrequests[0][1] == 1;");
            sb.AppendLine("#assert LiftSystem() |= []<> off; ");
            sb.AppendLine("#assert LiftSystem() |= [](on -> <>off) && [](on2 -> <>moving.1.0.1); ");

            return sb.ToString();
        }


        public static string LoadKeylessCarSystem(int userNumber)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/* ");
            sb.AppendLine("One of the latest automotive technologies, push-button keyless system, allows you to start");
            sb.AppendLine("your car's engine without the hassle of key insertion and offers great convenience. ");
            sb.AppendLine();
            sb.AppendLine("Push-button keyless system allows owner with key-fob in her pocket to unlock the door when she is very near the car.");
            sb.AppendLine("The driver can slide behind the wheel, with the key-fob in her pocket ");
            sb.AppendLine("(briefcase or purse or anywhere inside the car), she can push the start/stop button on the control panel. ");
            sb.AppendLine("Shutting off the engine is just as hassle-free, and is accomplished by merely pressing the start/stop button. ");
            sb.AppendLine();
            sb.AppendLine("These systems are designed so it is impossible to start the engine without the owner's key-fob");
            sb.AppendLine("and it cannot lock your key-fob inside the car because the system will sense it and prevent ");
            sb.AppendLine("the user from locking them in.");
            sb.AppendLine();
            sb.AppendLine("However, the keyless system can also surprise you as it may allow you to drive the car without key-fob.");
            sb.AppendLine("This has happened to Jin Song when his wife droped him to his office on the way to a shopping mall but");
            sb.AppendLine("the key-fob was in Jin Song's pocket. At the shopping mall, when the engine was turned off, the car could");
            sb.AppendLine("not be locked or re-started again. Jin Song had to take a taxi to the mall to pass the key-fob.");
            sb.AppendLine("This really motivated Jin Song to come up the model :-)");
            sb.AppendLine("*/");
            sb.AppendLine();
            sb.AppendLine("#define N "+userNumber+";        // number of owners");
            sb.AppendLine();
            sb.AppendLine("//enumerations used in the model");
            sb.AppendLine("#define far 0;      // owner is out and far away from the car");
            sb.AppendLine("#define near 1;     // owner is near and close enough to open/lock the door if he/she has the keyfob");
            sb.AppendLine("#define in 2;       // owner is in the car");
            sb.AppendLine();
            sb.AppendLine("#define off 0;      // engine is off");
            sb.AppendLine("#define on 1;       // engine is on");
            sb.AppendLine();
            sb.AppendLine("#define unlock 0;   // door is unlocked but closed");
            sb.AppendLine("#define lock 1;     // door is locked (must be closed)");
            sb.AppendLine("#define open 2;     // door is open");
            sb.AppendLine();
            sb.AppendLine("#define incar -1;    // key is put inside car");
            sb.AppendLine("#define faralone -2; // key is put outside and far");
            sb.AppendLine();
            sb.AppendLine("var owner[N];       // owners' position. initially, all users are far away from the car");
            sb.AppendLine();
            sb.AppendLine("var engine = off;   // engine status, initially off");
            sb.AppendLine("var door = lock;    // door status, initially locked");
            sb.AppendLine("var key = 0;        // key fob position, initially, it is with first owner");
            sb.AppendLine("var moving = 0;     // car moving status, 0 for stop and 1 for moving");
            sb.AppendLine("var fuel = 10;      // energy costs, say 1 for a short drive and 5 for a long driving");
            sb.AppendLine();
            sb.AppendLine("owner_pos(i) = ");
            sb.AppendLine("          [owner[i] == far]towards.i{owner[i] = near;} -> owner_pos(i)");
            sb.AppendLine("          []");
            sb.AppendLine("          [owner[i] == near]goaway.i{owner[i] = far;} -> owner_pos(i)");
            sb.AppendLine("          []");
            sb.AppendLine("          [owner[i] == near && door == open && moving==0]getin.i{owner[i] = in; } -> owner_pos(i)");
            sb.AppendLine("          []");
            sb.AppendLine("          [owner[i] == in && door == open && moving==0]goout.i{owner[i] = near; } -> owner_pos(i);          ");
            sb.AppendLine();
            sb.AppendLine("key_pos(i) = ");
            sb.AppendLine("	         [key == i && owner[i] == in]putincar.i{key = incar;} -> key_pos(i) ");
            sb.AppendLine("          [] ");
            sb.AppendLine("          [key == i && owner[i] == far]putaway.i{key = faralone;} -> key_pos(i)");
            sb.AppendLine("          []");
            sb.AppendLine("          [(key == faralone && owner[i] == far) || (key == incar && owner[i] == in)]getkey.i{key = i;} -> key_pos(i);          ");
            sb.AppendLine();
            sb.AppendLine("door_op(i) = ");
            sb.AppendLine("          [key == i && owner[i]==near && door ==lock && moving==0]unlockopen.i{door = open;} -> door_op(i)");
            sb.AppendLine("          []");
            sb.AppendLine("          [owner[i]==near && door==unlock && moving==0]justopen.i{door = open;} -> door_op(i)");
            sb.AppendLine("          []");
            sb.AppendLine("          [door != open && owner[i] == in]insideopen.i{door = open;} -> door_op(i)");
            sb.AppendLine("          []");
            sb.AppendLine("          [door == open]close.i{door = unlock;} -> door_op(i)");
            sb.AppendLine("          []");
            sb.AppendLine("          [door==unlock&&owner[i]==in]insidelock.i{door=lock;} -> door_op(i)");
            sb.AppendLine("          []");
            sb.AppendLine("          [door == unlock && owner[i]==near && key==i]outsidelock.i{door=lock;} -> door_op(i);");
            sb.AppendLine("        ");
            sb.AppendLine("motor(i) = ");
            sb.AppendLine("          [owner[i]==in&&(key==i||key==incar)&&engine==off&& fuel!= 0]turnon.i{engine = on;} -> motor(i)");
            sb.AppendLine("          []");
            sb.AppendLine("          [engine==on&&owner[i]==in&&moving==0]startdrive.i{moving=1;} -> motor(i) ");
            sb.AppendLine("          []");
            sb.AppendLine("          [moving==1&&fuel!=0]shortdrive.i{fuel=fuel-1;if (fuel==0) {engine=off; moving =0;}  } -> motor(i)");
            sb.AppendLine("          []");
            sb.AppendLine("          [moving==1&&fuel > 5]longdrive.i{fuel=fuel-5;if (fuel==0) {engine=off; moving =0;}} -> motor(i)");
            sb.AppendLine("          []");
            sb.AppendLine("          [engine==on&&moving==1&&owner[i]==in]stop.i{moving=0;} -> motor(i)");
            sb.AppendLine("          []");
            sb.AppendLine("          [fuel==0&&engine==off]refill{fuel=10;} -> motor(i)");
            sb.AppendLine("          [] ");
            sb.AppendLine("          [engine==on&&moving==0&&owner[i]==in]turnoff.i{engine = off;} -> motor(i);");
            sb.AppendLine("          ");
            sb.AppendLine("car =     (||i:{0..N-1} @ (motor(i) || door_op(i) || key_pos(i) || owner_pos(i)));");
            sb.AppendLine();
            sb.AppendLine("#define runwithoutowner (moving==1" + GetOwnerString(userNumber, "== far") + "); //can car moving without owner");
            sb.AppendLine("#define ownerdrivetogether (moving==1" + GetOwnerString(userNumber, "== in") + "); //can ower drives the car");
            sb.AppendLine("#define keylockinside (key == incar && door == lock" + GetOwnerString(userNumber, "!= in") + "); // can key be locked inside");
            sb.AppendLine("#define drivewithoutengineon (moving==1 && engine==off); //can car move when engine is off");
            sb.AppendLine("#define drivewithoutfuel (moving==1&&fuel==0); //can car move without fuel");
            sb.AppendLine("#define drivewithoutkeyholdbyother (moving ==1 && owner[1] == in && owner[0] == far && key == 0); //car can be driven without key");
            sb.AppendLine();
            sb.AppendLine("#assert car deadlockfree;");
            sb.AppendLine("#assert car |= []<> longdrive.0;");
            sb.AppendLine("#assert car reaches keylockinside;");
            sb.AppendLine("#assert car reaches runwithoutowner;");
            sb.AppendLine("#assert car reaches ownerdrivetogether;");
            sb.AppendLine("#assert car reaches drivewithoutengineon;");
            sb.AppendLine("#assert car reaches drivewithoutfuel;");
            sb.AppendLine("#assert car reaches drivewithoutkeyholdbyother;");

            return sb.ToString();

        }

        private static string GetOwnerString(int users, string action)
        {
            string result = "";
            for (int i = 0; i < users; i++)
            {
                result += " && owner[" + i + "] " + action;

            }
            return result;
        }


        public static string LoadBuyerModel(int buyerNumber, bool isConnected)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("//The buyer communicates with the seller through the service channel B2S to invoke its service.");
            sb.AppendLine("//Channel Bch which is sent along the message is to be used as a private channel for the following session only.");
            sb.AppendLine("//In the Session, the buyer firstly sends a message QuoteRequest through channel Bch to the seller.");
            sb.AppendLine("//The seller responds with some quotation value x, which is a variable.");
            sb.AppendLine();
            sb.AppendLine("//Notice that in choreography, the value of x is left unspecified at this point.");
            sb.AppendLine("//The seller sells a message through the service channel S2H to invoke a shipping service.");
            sb.AppendLine();
            sb.AppendLine("//Notice that the channel Bch is passed onto the shipper so that the shipper may contact the buyer directly.");
            sb.AppendLine();
            sb.AppendLine("channel B2S ; //The pre-established channel between the buyer and the seller");
            sb.AppendLine("channel S2H ; //The pre-established channel between the seller and the shipper");
            sb.AppendLine();
            sb.AppendLine("//the following is the specification: choreography");
            if (isConnected)
            {
                sb.AppendLine("Chor cBuySell {");
                sb.AppendLine("Main() =  B2S(Buyer, Seller, {Bch})  -> Bch(Seller, Buyer, Ack) -> Session();");
                sb.AppendLine();
                sb.AppendLine("Session() = Bch(Buyer, Seller, QuoteRequest) -> ");
                sb.AppendLine("	Bch(Seller, Buyer, QuoteResponse.x) -> ");
                sb.AppendLine("	if (x <= 1000) {");
                sb.AppendLine("		Bch(Buyer, Seller, QuoteAccept) -> ");
                sb.AppendLine("		Bch(Seller, Buyer, OrderConfirmation) ->");
                sb.AppendLine("		Bch(Buyer, Seller, HiThere) ->");                
                sb.AppendLine("		S2H(Seller, Shipper, {Bch, Sch}) -> ");
                sb.AppendLine("		(Sch(Shipper, Buyer, DeliveryDetails.y) -> Stop");
                sb.AppendLine("			||| ");
                sb.AppendLine("		 Bch(Shipper, Seller, DeliveryDetails.z) -> Stop)");
                sb.AppendLine("	}");
                sb.AppendLine("	else {");
                sb.AppendLine("		Bch(Buyer, Seller, QuoteReject) -> Bch(Seller, Buyer, HiThere) -> Session()");
                sb.AppendLine("		[] Bch(Buyer, Seller, Terminate) -> Stop");
                sb.AppendLine("	};");
                sb.AppendLine("}");
            }
            else
            {
                sb.AppendLine("Chor cBuySell {");
                sb.AppendLine("Main() =  B2S(Buyer, Seller, {Bch})  -> Bch(Seller, Buyer, Ack) -> Session();");
                sb.AppendLine();
                sb.AppendLine("Session() = Bch(Buyer, Seller, QuoteRequest) -> ");
                sb.AppendLine("	Bch(Seller, Buyer, QuoteResponse.x) -> ");
                sb.AppendLine("	if (x <= 1000) {");
                sb.AppendLine("		Bch(Buyer, Seller, QuoteAccept) -> ");
                sb.AppendLine("		Bch(Seller, Buyer, OrderConfirmation) ->");
                sb.AppendLine("		S2H(Seller, Shipper, {Bch, Sch}) -> ");
                sb.AppendLine("		(Sch(Shipper, Buyer, DeliveryDetails.y) -> Stop");
                sb.AppendLine("			||| ");
                sb.AppendLine("		 Bch(Shipper, Seller, DeliveryDetails.z) -> Stop)");
                sb.AppendLine("	}");
                sb.AppendLine("	else {");
                sb.AppendLine("		Bch(Buyer, Seller, QuoteReject) -> Session()");
                sb.AppendLine("		[] Bch(Buyer, Seller, Terminate) -> Stop");
                sb.AppendLine("	};");
                sb.AppendLine("}");    
            }

            
            sb.AppendLine();
            sb.AppendLine("//the following is the implementation: orchstration");
            sb.AppendLine();
            sb.AppendLine("Orch oBuySell {	");

            for (int i = 1; i < buyerNumber+1; i++)
            {
                sb.AppendLine("Role Buyer"+i+" {");
                sb.AppendLine("    var counter = 0;");
                sb.AppendLine();
                sb.AppendLine("    Main () = {counter = 0} -> B2S!{Bch" + i + "} -> Bch" + i + "?Ack -> Session();");
                sb.AppendLine();
                sb.AppendLine("    Session() = Bch"+i+"!QuoteRequest -> ");
                sb.AppendLine("    {counter = counter+1} ->");
                sb.AppendLine("	Bch" + i + "?QuoteResponse.x -> ");
                sb.AppendLine("	if (x <= 1000) {");
                sb.AppendLine("		Bch" + i + "!QuoteAccept -> ");
                sb.AppendLine("		Bch" + i + "?OrderConfirmation -> ");
                sb.AppendLine("		Bch" + i + "?DeliveryDetails.y ->");
                sb.AppendLine("		Stop");
                sb.AppendLine("	}");
                sb.AppendLine("	else {");
                sb.AppendLine("		if (counter <= 3) {");
                sb.AppendLine("			Bch" + i + "!QuoteReject ->");
                sb.AppendLine("			Session()				");
                sb.AppendLine("		}");
                sb.AppendLine("		else {");
                sb.AppendLine("			Bch" + i + "!Terminate ->");
                sb.AppendLine("			Main");
                sb.AppendLine("		}");
                sb.AppendLine("	};");
                sb.AppendLine("}");
                sb.AppendLine();
            }

            sb.AppendLine("Role Seller[" + buyerNumber + "] {");
            sb.AppendLine("    var x = 1200; //the quotation");
            sb.AppendLine();
            sb.AppendLine("    Main =  B2S?{ch} -> ch!Ack -> Session();");
            sb.AppendLine();
            sb.AppendLine("    Session() = ch?QuoteRequest -> ");
            sb.AppendLine("	ch!QuoteResponse.x ->");
            sb.AppendLine("	(");
            sb.AppendLine("		ch?QuoteAccept ->");
            sb.AppendLine("		ch!OrderConfirmation ->");
            sb.AppendLine("		S2H!{ch,Sch} ->");
            sb.AppendLine("		Sch?DeliveryDetails.y ->");
            sb.AppendLine("		Stop");
            sb.AppendLine("	[]");
            sb.AppendLine("		( ch?QuoteReject -> Session()");
            sb.AppendLine("		[]");
            sb.AppendLine("		 ch?Terminate -> Stop)");
            sb.AppendLine("	);");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("Role Shipper[" + buyerNumber + "] {");
            sb.AppendLine("    var detail = 2010; //the delivery detail");
            sb.AppendLine();
            sb.AppendLine("    Main() =  S2H?{ch1,ch2} -> (");
            sb.AppendLine("	ch1!DelieryDetails.detail -> Stop |||");
            sb.AppendLine("	ch2!DelieryDetails.detail -> Stop);");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("#assert oBuySell deadlockfree;");
            sb.AppendLine("#assert oBuySell refines cBuySell; ");
            sb.AppendLine("#define inv Buyer1.counter <= 4;");
            sb.AppendLine("#assert oBuySell |= []inv;");
            sb.AppendLine("#assert oBuySell |= []<> inv;");
            sb.AppendLine("#assert oBuySell reaches inv; ");

            return sb.ToString();
        }

        public static string LoadRoadAssistanceModel()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("channel clientAssistant 1;");
            sb.AppendLine("channel assistantBank 1;");
            sb.AppendLine("channel assistantGPS 1;");
            sb.AppendLine("channel assistantUDDI 1;");
            sb.AppendLine("channel assistantReasoner 1;");
            sb.AppendLine("channel assistantGarage 1;");
            sb.AppendLine("channel assistantRecovery 1;");

            sb.AppendLine();
            sb.AppendLine("Chor cRoadAssistance {");
            sb.AppendLine("Main() = RequestAssistance(); (RequireProduct() ||| (GetLocation(); GetLocalServices())); ");
            sb.AppendLine("	SelectServices(); OrderGarage(); (GetRecovery() ||| GetRentalCar());");
            sb.AppendLine();
            sb.AppendLine("RequestAssistance() = clientAssistant(Client, RoadAssistance, {ch1}) ->");
            sb.AppendLine("		ch1(Client, RoadAssistance, request_bookProfile) -> ");
            sb.AppendLine("		ch1(RoadAssistance, Client, respond_bookAvailability) -> Skip; ");
            sb.AppendLine();
            sb.AppendLine("RequireProduct() = assistantBank(RoadAssistance,Bank,{ch2}) ->");
            sb.AppendLine("		ch2(RoadAssistance,Bank,request_bookProfile) ->");
            sb.AppendLine("		ch2(Bank,RoadAssistance,respond_bookAvailability) -> Skip;");
            sb.AppendLine();
            sb.AppendLine("GetLocation() = assistantGPS(RoadAssistance,GPS,{ch3}) ->");
            sb.AppendLine("		ch3(RoadAssistance,GPS,request_bookProfile) ->");
            sb.AppendLine("		ch3(GPS,RoadAssistance,respond_bookAvailability) -> Skip;");
            sb.AppendLine();
            sb.AppendLine("GetLocalServices() = assistantUDDI(RoadAssistance,uddilocal,{ch4}) ->");
            sb.AppendLine("		ch4(RoadAssistance,uddilocal, request_bookProfile) -> ");
            sb.AppendLine("		ch4(uddilocal, RoadAssistance, request_bookAvailability) -> Skip;");
            sb.AppendLine();
            sb.AppendLine("SelectServices() = assistantReasoner(RoadAssistance, Reasoner,{ch5}) ->");
            sb.AppendLine("		ch5(RoadAssistance,Reasoner,request_bookProfile) ->");
            sb.AppendLine("		ch5(Reasoner,RoadAssistance,request_bookAvailability) -> Skip;");
            sb.AppendLine();
            sb.AppendLine("OrderGarage() = assistantGarage(RoadAssistance, Garage,{ch6}) ->");
            sb.AppendLine("		ch6(RoadAssistance,Garage,request_bookProfile) ->");
            sb.AppendLine("		ch6(Garage,RoadAssistance,request_bookAvailability) -> Skip;");
            sb.AppendLine();
            sb.AppendLine("GetRecovery() = assistantRecovery(RoadAssistance, Recovery,{ch7}) ->");
            sb.AppendLine("		ch7(RoadAssistance,Recovery,request_bookProfile) ->");
            sb.AppendLine("		ch7(Recovery,RoadAssistance,request_bookAvailability) -> Skip;");
            sb.AppendLine();
            sb.AppendLine("GetRentalCar() = assistantRecovery(RoadAssistance, CarRental,{ch8}) ->");
            sb.AppendLine("		ch8(RoadAssistance,CarRental,request_bookProfile) ->");
            sb.AppendLine("		ch8(CarRental,RoadAssistance,request_bookAvailability) -> Skip;");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("Orch oBuySellRoadAssistance {");
            sb.AppendLine("	Role RoadAssistance {");
            sb.AppendLine("		Main() = Stop;");
            sb.AppendLine("	}");
            sb.AppendLine();
            sb.AppendLine("	Role Client {");
            sb.AppendLine("		Main() = Stop;");
            sb.AppendLine("	}");
            sb.AppendLine("}");
            sb.AppendLine("");

            return sb.ToString();
        }


        public static string LoadTravelAgent(int buyerNumber, bool isDeadlock)
        {
            StringBuilder sb = new StringBuilder();
            string endPoint = isDeadlock ? "Stop()" : "Session()";

            sb.AppendLine("channel C2A ; //The pre-established channel between the seller and the shipper");
            sb.AppendLine("channel A2E ; //The pre-established channel between the seller and the shipper");
            sb.AppendLine("channel A2A ; //The pre-established channel between the seller and the shipper");
            sb.AppendLine("channel A2D ; //The pre-established channel between the seller and the shipper");

            sb.AppendLine("//the following is the specification: choreography");
            sb.AppendLine("Chor cTravelProcess {");
            sb.AppendLine("Main() = C2A(Client, TravelAgent, {Bch}) -> Bch(TravelAgent, Client, Ack) -> Session();");

            sb.AppendLine("Session() = Bch(Client, TravelAgent, TravelRequest) -> ");
            sb.AppendLine("	A2E(TravelAgent, Employee, {Ech}) ->Ech(Employee, TravelAgent, Ack) -> Ech(TravelAgent, Employee, EmployeeTravelStatusRequest) -> Ech(Employee, TravelAgent, EmployeeTravelStatusResponse.class) ->	A2A(TravelAgent, AmericanAirlines, {ch1}) -> ch1(AmericanAirlines, TravelAgent, Ack) -> A2D(TravelAgent, DeltaAirlines, {ch2}) -> ch2(DeltaAirlines, TravelAgent, Ack) -> ");
            sb.AppendLine("	((");
            sb.AppendLine("	ch1(TravelAgent, AmericanAirlines, FlightAvailabilityRequest) -> ch1(AmericanAirlines, TravelAgent, FlightResponseAA.price1) -> {TravelAgent.p1 = price1}-> Skip");
            sb.AppendLine("	|||");
            sb.AppendLine("	ch2(TravelAgent, DeltaAirlines, FlightAvailabilityRequest) -> ch2(DeltaAirlines, TravelAgent, FlightResponseDA.price2) -> {TravelAgent.p2 = price2}-> Skip	");
            sb.AppendLine("	);");
            sb.AppendLine("	if (TravelAgent.p1 > TravelAgent.p2) {");
            sb.AppendLine("		Bch(TravelAgent, Client, TravelResponse.x) -> 	Bch(Client, TravelAgent, ClientAck) -> ch1(TravelAgent, AmericanAirlines, FlightACK) -> ch1(AmericanAirlines, TravelAgent, EndACK) -> ch2(TravelAgent, DeltaAirlines, FlightACK) -> ch2(DeltaAirlines, TravelAgent, EndACK) -> Bch(TravelAgent, Client, EndAck) -> " + endPoint);
            sb.AppendLine("	}");
            sb.AppendLine("	else {");
            sb.AppendLine("		Bch(TravelAgent, Client, TravelResponse.y) -> 	Bch(Client, TravelAgent, ClientAck) -> ch1(TravelAgent, AmericanAirlines, FlightACK) -> ch1(AmericanAirlines, TravelAgent, EndACK) -> ch2(TravelAgent, DeltaAirlines, FlightACK) -> ch2(DeltaAirlines, TravelAgent, EndACK) -> Bch(TravelAgent, Client, EndAck) -> " + endPoint);
            sb.AppendLine("	});");
            sb.AppendLine("}");

            sb.AppendLine("//the following is the implementation: orchstration");
            sb.AppendLine("Orch oTravelProcess {	");

            for (int i = 0; i < buyerNumber; i++)
            {
                sb.AppendLine("Role Client" + i);
                sb.AppendLine("{");
                sb.AppendLine("	Main() = C2A!{Bch} -> Bch?Ack -> Session();");
                sb.AppendLine("	Session() = Bch!TravelRequest -> Bch?TravelResponse.price -> Bch!ClientAck -> Bch?EndAck -> " + endPoint + ";");
                sb.AppendLine("}");
            }

            sb.AppendLine("Role TravelAgent[" + buyerNumber + "] {");
            sb.AppendLine("	var p1 = 0;");
            sb.AppendLine("	var p2 = 0;");
            sb.AppendLine("	Main() = C2A?{Bch} ->  Bch!Ack -> Session();");
            sb.AppendLine("	Session() = Bch?TravelRequest ->  A2E!{Ech} ->  Ech?Ack -> Ech!EmployeeTravelStatusRequest -> Ech?EmployeeTravelStatusResponse.class -> A2A!{ch1} -> ch1?Ack -> A2D!{ch2} -> ch2?Ack ->");
            sb.AppendLine("	    ((");
            sb.AppendLine("	      ch1!FlightAvailabilityRequest -> ch1?FlightResponseAA.price1  -> {p1 = price1} -> Skip");
            sb.AppendLine("	    |||");
            sb.AppendLine("	      ch2!FlightAvailabilityRequest -> ch2?FlightResponseDA.price2 -> {p2 = price2} -> Skip");
            sb.AppendLine("	    );");
            sb.AppendLine("	    if(p1 > p2)");
            sb.AppendLine("	    {");
            sb.AppendLine("	    	Bch!TravelResponse.p2 -> Bch?ClientAck -> ch1!FlightACK -> ch1?EndACK  -> ch2!FlightACK -> ch2?EndACK -> Bch!EndAck ->  " + endPoint);
            sb.AppendLine("	    }");
            sb.AppendLine("	    else");
            sb.AppendLine("	    {");
            sb.AppendLine("	    	Bch!TravelResponse.p1 -> Bch?ClientAck -> ch1!FlightACK -> ch1?EndACK -> ch2!FlightACK -> ch2?EndACK -> Bch!EndAck ->  " + endPoint);
            sb.AppendLine("	    });");
            sb.AppendLine("}");
            sb.AppendLine("Role Employee[" + buyerNumber + "] {");
            sb.AppendLine("	Main() = A2E?{Ech} -> Ech!Ack ->  Ech?EmployeeTravelStatusRequest -> Ech!EmployeeTravelStatusResponse.0 -> Stop();");
            sb.AppendLine("}");
            sb.AppendLine("Role AmericanAirlines[" + buyerNumber + "] {");
            sb.AppendLine("	Main() = A2A?{ch} -> ch!Ack -> ch?FlightAvailabilityRequest -> ch!FlightResponseAA.120 -> ch?FlightACK -> ch!EndACK -> Stop();");
            sb.AppendLine("}");
            sb.AppendLine("Role DeltaAirlines[" + buyerNumber + "] {");
            sb.AppendLine("	Main() = A2D?{ch} ->ch!Ack ->   ch?FlightAvailabilityRequest -> ch!FlightResponseDA.130 -> ch?FlightACK -> ch!EndACK -> Stop();");
            sb.AppendLine("}");
            sb.AppendLine("}");
            sb.AppendLine("#assert oTravelProcess deadlockfree;");
            sb.AppendLine("#assert oTravelProcess refines cTravelProcess; ");




            return sb.ToString();
        }

        /// <summary>
        /// This method loads the classic Bridge crossing example. 
        /// </summary>
        /// <param name="upbound">
        /// unbound is the maximum number of minutes need to be examined before the search stop; e.g., 18 means if 18 has elasped, stop.
        /// </param>
        /// <returns></returns>
        public static string LoadBridgePuzzle(int upbound)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("//The classic Bridge Crossing Puzzle");
            sb.AppendLine();

            sb.AppendLine("////////////////The Model//////////////////");
            sb.AppendLine("#define Max " + upbound + ";");
            sb.AppendLine();

            sb.AppendLine("#define KNIGHT 1;");
            sb.AppendLine("#define LADY 2;");
            sb.AppendLine("#define KING 5;");
            sb.AppendLine("#define QUEEN 10;");
            sb.AppendLine();
            sb.AppendLine("var Knight = 0; ");
            sb.AppendLine("var Lady = 0; ");
            sb.AppendLine("var King = 0; ");
            sb.AppendLine("var Queen = 0; ");
            sb.AppendLine("var time; ");
            sb.AppendLine();
            sb.AppendLine("South() = [time <= Max && Knight == 0 && Lady == 0]go_knight_lady{Knight = 1; Lady= 1; time = time+LADY;} -> North()");
            sb.AppendLine("                 [] [time <= Max && Knight == 0 && King == 0]go_knight_king{Knight = 1; King= 1; time = time+KING;} -> North()        ");
            sb.AppendLine("                 [] [time <= Max && Knight == 0 && Queen == 0]go_knight_queen{Knight = 1; Queen= 1; time = time+QUEEN;} -> North()");
            sb.AppendLine("                 [] [time <= Max && Lady == 0 && King == 0]go_lady_king{Lady = 1; King= 1; time = time+KING;} -> North()");
            sb.AppendLine("                 [] [time <= Max && Lady == 0 && Queen == 0]go_lady_queen{Lady = 1; Queen= 1; time = time+QUEEN;} -> North()");
            sb.AppendLine("                 [] [time <= Max && King == 0 && Queen == 0]go_king_queen{King = 1; Queen= 1; time = time+QUEEN;} -> North()");
            sb.AppendLine("                 [] [time <= Max && Knight == 0]go_knight{Knight = 1; time = time+KNIGHT;} -> North()");
            sb.AppendLine("                 [] [time <= Max && Lady == 0]go_lady{Lady = 1; time = time+LADY;} -> North()");
            sb.AppendLine("                 [] [time <= Max && King == 0]go_king{King = 1; time = time+KING;} -> North()");
            sb.AppendLine("                 [] [time <= Max && Queen == 0]go_queen{Queen = 1; time = time+QUEEN;} -> North();");
            sb.AppendLine();
            sb.AppendLine("North() = [time <= Max && Knight == 1 && Lady == 1]back_knight_lady{Knight = 0; Lady = 0; time = time+LADY;} -> South()");
            sb.AppendLine("                [] [time <= Max && Knight == 1 && King == 1]back_knight_king{Knight = 0; King = 0; time = time+KING;} -> South()");
            sb.AppendLine("                [] [time <= Max && Knight == 1 && Queen == 1]back_knight_queen{Knight = 0; Queen = 0; time = time+QUEEN;} -> South()");
            sb.AppendLine("                [] [time <= Max && Lady == 1 && King == 1]back_lady_king{Lady = 0; King = 0; time = time+KING;} -> South()");
            sb.AppendLine("                [] [time <= Max && Lady == 1 && Queen == 1]back_lady_queen{Lady = 0; Queen = 0; time = time+QUEEN;} -> South()");
            sb.AppendLine("                [] [time <= Max && King == 1 && Queen == 1]back_king_queen{King = 0; Queen = 0; time = time+QUEEN;} -> South()");
            sb.AppendLine("                [] [time <= Max && Knight == 1]back_knight{Knight = 0; time = time+KNIGHT;} -> South()");
            sb.AppendLine("                [] [time <= Max && Lady == 1]back_lady{Lady = 0; time = time+LADY;} -> South()");
            sb.AppendLine("                [] [time <= Max && King == 1]back_king{King = 0; time = time+KING;} -> South()");
            sb.AppendLine("                [] [time <= Max && Queen == 1]back_queen{Queen = 0; time = time+QUEEN;} -> South();");
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine("////////////////The Properties//////////////////");
            sb.AppendLine("#define goal (Knight==1 && Lady==1 && King==1 && Queen==1);");
            //sb.AppendLine("#define untimed_goal (Knight==1 && Lady==1 && King==1 && Queen==1);");
            sb.AppendLine("#define alter ((Knight==1 && Lady==1 && King==1 && Queen==1 && time > Max) || !(Knight==1 && Lady==1 && King==1 && Queen==1));");
            sb.AppendLine();

            sb.AppendLine("#assert South() reaches goal;");
            sb.AppendLine("//If the time is unknow, we can ask for the minimum time needed to reach the goal.");
            sb.AppendLine("#assert South() reaches goal with min(time);");
            sb.AppendLine("//Alternative way to query the reachibility using LTL, PAT will recognize the safety LTL and turn it into reachibility test.");
            sb.AppendLine("#assert South() |= [] alter;");
            //sb.Append(Ultility.Ultility.DESCRPTION_SEPARATOR);
            //sb.AppendLine("All four people start out on the southern side of the bridge, namely the king, queen, a young lady and a knight. The goal is for everyone to arrive at the castle north of the bridge before the time runs out. The bridge can hold at most two people at a time and they must be carrying the torch when crossing the bridge. The king needs 5 minutes to cross, the queen 10 minutes, the lady 2 minutes and the night 1 minutes.");

            return sb.ToString();
        }


        public static string LoadMissionariesCannibals(int N, int M)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("//Missionaries and Cannibals Problem");
            sb.AppendLine("//Please do not change the value of M here. If you want to change M, please use the menubar to reopen Examples");
            sb.AppendLine();

            sb.AppendLine("////////////////The Model//////////////////");
            sb.AppendLine("//The number of Missionary or the number of Cannibal");
            sb.AppendLine("#define N " + N + ";");
            sb.AppendLine();

            sb.AppendLine("//the number of people that the boat can hold");
            sb.AppendLine("#define M " + M + ";");
            sb.AppendLine();

            sb.AppendLine("var missionary=N;");
            sb.AppendLine("var cannibal=N;");
            sb.AppendLine();

            //-----------------
            sb.AppendLine("Cross()=[cannibal>=1 && (missionary==0 || missionary==N)]one_cannibal_cross{cannibal=cannibal-1;}->Return()");
            sb.AppendLine("     [] [cannibal>=2 && (missionary==0 || missionary==N|)]two_cannibals_cross{cannibal=cannibal-2;}->Return()");
            if (M >= 3) sb.AppendLine("     [] [cannibal>=3 && (missionary==0 || missionary==N|)]three_cannibals_cross{cannibal=cannibal-3;}->Return()");
            if (M == 4) sb.AppendLine("     [] [cannibal>=4 && (missionary==0 || missionary==N|)]four_cannibals_cross{cannibal=cannibal-4;}->Return()");
            sb.AppendLine("     [] [missionary-cannibal==1 || missionary==1]one_missionary_cross{missionary=missionary-1;}->Return()");
            sb.AppendLine("     [] [missionary-cannibal==2 || missionary==2]two_missionaries_cross{missionary=missionary-2;}->Return()");
            if (M >= 3) sb.AppendLine("     [] [(missionary-cannibal==3 || missionary==3)]three_missionaries_cross{missionary=missionary-3;}->Return()");
            if (M == 4) sb.AppendLine("     [] [(missionary-cannibal==4 || missionary==4)]four_missionaries_cross{missionary=missionary-4;}->Return()");
            if (M == 4) sb.AppendLine("     [] [cannibal==3 && missionary==3]three_missionaries_one_cannibal_cross{missionary=missionary-3;cannibal=cannibal-1;}->Return()");
            if (M >= 3) sb.AppendLine("     [] [cannibal==2 && missionary==2]two_missionaries_one_cannibal_cross{missionary=missionary-2;cannibal=cannibal-1;}->Return()");
            if (M == 4) sb.AppendLine("     [] [cannibal>=2 && missionary>=2]two_missionaries_two_cannibals_cross{missionary=missionary-2;cannibal=cannibal-2;}->Return()");
            sb.AppendLine("     [] [cannibal>=1 && missionary>=1]one_missionary_one_cannibal_cross{missionary=missionary-1;cannibal=cannibal-1;}->Return();");
            //------------------------------
            sb.AppendLine();
            sb.AppendLine("Return()= [cannibal<=N-1 &&(missionary==0||missionary==N) ]one_cannibal_return{cannibal=cannibal+1;}->Cross()");
            sb.AppendLine("     [] [cannibal<=N-2&&(missionary==0||missionary==N)]two_cannibals_return{cannibal=cannibal+2;}->Cross()");
            if (M >= 3) sb.AppendLine("     [] [cannibal<=N-3&&(missionary==0||missionary==N)]three_cannibals_return{cannibal=cannibal+3;}->Cross()");
            if (M == 4) sb.AppendLine("     [] [cannibal<=N-4&&(missionary==0||missionary==N)]four_cannibals_return{cannibal=cannibal+4;}->Cross()");
            sb.AppendLine("     [] [missionary==N-1]one_missionary_return{missionary=missionary+1;}->Cross()");
            sb.AppendLine("     [] [missionary==N-2]two_missionaries_return{missionary=missionary+2;}->Cross()");
            if (M >= 3) sb.AppendLine("     [] [missionary==N-3]three_missionaries_return{missionary=missionary+3;}->Cross()");
            if (M == 4) sb.AppendLine("     [] [missionary==N-4]four_missionaries_return{missionary=missionary+4;}->Cross()");
            if (M == 4) sb.AppendLine("     [] [cannibal==N-3 && missionary==N-3]three_missionaries_one_cannibal_return{missionary=missionary+3;cannibal=cannibal+1;}->Cross()");
            if (M >= 3) sb.AppendLine("     [] [cannibal==N-2 && missionary==N-2]two_missionaries_one_cannibal_return{missionary=missionary+2;cannibal=cannibal+1;}->Cross()");
            if (M == 4) sb.AppendLine("     [] [cannibal<=N-2 && missionary<=N-2]two_missionaries_two_cannibals_return{missionary=missionary+2;cannibal=cannibal+2;}->Cross()");
            sb.AppendLine("     [] [cannibal<=N-1 && missionary<=N-1]one_missionary_one_cannibal_return{missionary=missionary+1;cannibal=cannibal+1;}->Cross();");



            sb.AppendLine();
            sb.AppendLine("////////////////The Properties//////////////////");
            sb.AppendLine("#define goal (missionary==0 && cannibal==0);");
            sb.AppendLine("#assert Cross() reaches goal;");




            return sb.ToString();

        }

        public static string LoadWolfGoatCabbage()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("//Wolf Goat Cabbage Problem");
            sb.AppendLine();

            sb.AppendLine("////////////////The Model//////////////////");
            sb.AppendLine("var farmer=0;");
            sb.AppendLine("var wolf=0;");
            sb.AppendLine("var goat=0;");
            sb.AppendLine("var carbage=0;");

            sb.AppendLine();

            sb.AppendLine("Cross()=[farmer==0 && ((! (wolf==0 && goat==0) ) && (! (goat==0 && carbage==0)))]farmer_cross{farmer=1;}->Return()");
            sb.AppendLine("	[] [farmer==0 && wolf==0 && (! (goat==0 && carbage ==0))] farmer_wolf_cross{farmer=1;wolf=1;}->Return()");
            sb.AppendLine("	[] [farmer==0 && goat==0] farmer_goat_cross{farmer=1;goat=1;}->Return()");
            sb.AppendLine("	[] [farmer==0 && carbage==0 && (! (wolf==0 && goat==0))] farmer_carbage_cross{farmer=1;carbage=1;}->Return();");

            sb.AppendLine();

            sb.AppendLine("Return()=[farmer==1 && ((! (wolf==1 && goat==1)) && (! (goat==1 && carbage==1)))]farmer_return{farmer=0;}->Cross()");
            sb.AppendLine("	[] [farmer==1 && wolf==1 && (! (goat==1 && carbage ==1))] farmer_wolf_return{farmer=0;wolf=0;}->Cross()");
            sb.AppendLine("	[] [farmer==1 && goat==1] farmer_goat_return{farmer=0;goat=0;}->Cross()");
            sb.AppendLine("	[] [farmer==1 && carbage==1 && (! (wolf==1 && goat==1))] farmer_carbage_return{farmer=0;carbage=0;}->Cross();");
            sb.AppendLine();

            sb.AppendLine("////////////////The Properties//////////////////");
            sb.AppendLine("#define goal (farmer==1&&wolf==1&&goat==1&&carbage==1);");
            sb.AppendLine("#assert Cross() reaches goal;");
            return sb.ToString();

        }


        /// <summary>
        /// This exmaple is the classic Readers/Writeers Example.
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public static string LoadRW(int N, bool isFair)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("//The classic Readers/Writers Example model multiple processes accessing a shared file.");
            sb.AppendLine();

            sb.AppendLine("////////////////The Model//////////////////");
            sb.AppendLine("#define M " + N + ";");
            sb.AppendLine();

            if (isFair)
            {
                sb.AppendLine("Writer() \t= startwrite -> wf(stopwrite) -> Writer();");
                sb.AppendLine("Reader() \t= startread -> wf(stopread) -> Reader();");
            }
            else
            {
                sb.AppendLine("Writer() \t= startwrite -> stopwrite -> Writer();");
                sb.AppendLine("Reader() \t= startread -> stopread -> Reader();");
            }

            sb.AppendLine("Reading(i) \t= [i == 0]Controller() []");
            sb.AppendLine("\t \t \t \t [i == M] stopread -> Reading(i-1) []");
            sb.AppendLine("\t \t \t \t [i > 0 && i < M] (startread -> Reading(i+1) [] stopread -> Reading(i-1));");

            sb.AppendLine();
            sb.AppendLine("Controller() \t= startread -> Reading(1)");
            sb.AppendLine("\t \t \t \t [] stopread -> error -> Controller()");
            sb.AppendLine("\t \t \t \t [] startwrite -> (stopwrite -> Controller() [] stopread -> error -> Controller());");
            sb.AppendLine();

            sb.AppendLine("ReadersWriters() = Controller() || (|||x:{0..M-1} @ (Reader() ||| Writer()));");

            sb.AppendLine();
            sb.AppendLine("Implementation() \t= ReadersWriters() \\ {startread, stopread, startwrite, stopwrite};");
            sb.AppendLine("Specification() \t= error -> Specification();");
            sb.AppendLine();
            sb.AppendLine("#alphabet Reading {startread,stopread};");
            sb.AppendLine();

            sb.AppendLine("////////////////The Properties//////////////////");
            sb.AppendLine("#assert ReadersWriters() deadlockfree;");
            sb.AppendLine("#assert ReadersWriters() |= []<>error;");
            sb.AppendLine("#assert ReadersWriters() |= ![]<>error;");
            sb.AppendLine("#assert Implementation() refines Specification();");
            sb.AppendLine("#assert Specification() refines Implementation();");
            sb.AppendLine("#assert Implementation() refines <F> Specification();");
            sb.AppendLine("#assert Specification() refines <F> Implementation();");
            sb.AppendLine("#assert Implementation() refines <FD> Specification();");
            sb.AppendLine("#assert Specification() refines <FD> Implementation();");

            //sb.Append(Ultility.Ultility.DESCRPTION_SEPARATOR);
            //sb.AppendLine("In computer science, the readers-writers problems are examples of a common computing problem in concurrency. The problem deals with situations in which many threads must access the same shared memory at one time, some reading and some writing, with the natural constraint that no process may access the share for reading or writing while another process is in the act of writing to it. In particular, it is allowed for two readers to access the share at the same time. In this model, a controller is used to guarantee the correct coordination among multiple readers/writers. ");

            return sb.ToString();
        }

        /// <summary>
        /// This example is the classic Milner's Cyclic Scheduler.
        /// </summary>
        /// <param name="N"></param>
        /// <param name="isWL"></param>
        /// <returns></returns>
        public static string LoadMS(int N, bool isWL)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(
                "//The Milner's Cyclic Scheduler algorithm was published in Communication and Concurrency, 1989.");
            sb.AppendLine();
            sb.AppendLine("////////////////The Model//////////////////");
            sb.AppendLine("#define N " + N + ";");
            sb.AppendLine("#alphabet Cycle {ini.i,ini.(i+1)%N};");
            sb.AppendLine();

            if (!isWL)
            {
                sb.AppendLine(
                    "Cycle0 \t\t= a.0 -> ini.1 -> work.0 -> ini.0 -> Cycle0;");
                sb.AppendLine(
                    "Cycle(i) \t\t= ini.i -> a.i -> (atomic{work.i -> ini.(i+1)%N -> Skip}; Cycle(i) [] atomic{ini.(i+1)%N -> work.i -> Skip}; Cycle(i));");
            }
            else
            {
                sb.AppendLine(
                    "Cycle0 \t\t= a.0 -> ini.1 -> work.0 -> wl(ini.0) -> Cycle0;");
                sb.AppendLine(
                    "Cycle(i) \t\t= ini.i -> a.i -> (atomic{work.i -> wl(ini.(i+1)%N) -> Skip}; Cycle(i) [] atomic{wl(ini.(i+1)%N) -> work.i -> Skip}; Cycle(i));");
            }

            sb.AppendLine("MilnerAcyclic() = Cycle0 || (|| x:{1..N-1} @ Cycle(x)); ");

            string tmp = "ini.0,a.0";
            for (int i = 1; i < N; i++)
            {
                tmp += ",work." + i;
                tmp += ",ini." + i;
                tmp += ",a." + i;
            }
            
            sb.AppendLine("Implementation()=(MilnerAcyclic() \\ {" + tmp + "});");
            sb.AppendLine("Specification()=(work.0->Specification());");
            sb.AppendLine();

            sb.AppendLine("////////////////The Properties//////////////////");
            sb.AppendLine("#assert MilnerAcyclic() deadlockfree;");
            sb.AppendLine("#assert MilnerAcyclic() |= []<>work.0;");


            //tmp = "";
            //for (int i = 0; i < N; i++)
            //{
            //    tmp += "[]<>work." + i + " && ";
            //}

            //sb.AppendLine("#assert MilnerAcyclic() |= " + tmp.Substring(0, tmp.Length - 4) + ";");
            sb.AppendLine("#assert Implementation() refines Specification();");
            sb.AppendLine("#assert Specification() refines Implementation();");
            sb.AppendLine("#assert Implementation() refines <F> Specification();");
            sb.AppendLine("#assert Specification() refines <F> Implementation();");
            sb.AppendLine("#assert Implementation() refines <FD> Specification();");
            sb.AppendLine("#assert Specification() refines <FD> Implementation();");


            //sb.Append(Ultility.Ultility.DESCRPTION_SEPARATOR);
            //sb.AppendLine("This scheduling algorithm is described by Milner in \"in Communication and Concurrency, 1989\". There are N processes, which are activated in a cyclic manner, i.e., process i activates process i+1 and after process n process 1 is activated again. Moreover, a process may never be re-activated before it has terminated. The scheduler is built from n cyclers who are positioned in a ring. The first cycler plays a special role as it starts up the system. This model is perfect demonstration of the partial order reduction techniques for model checking.");

            return sb.ToString();
        }

        /// <summary>
        /// This example is the classic Dining Philosophiers. 
        /// </summary>
        /// <param name="N"></param>
        /// <param name="isWL"></param>
        /// <param name="isAllFair"></param>
        /// <param name="hasThink"></param>
        /// <returns></returns>
        public static string LoadDP(int N, bool isWL, bool isAllFair, bool hasThink, bool isLiveEvent)
        {
            string hasThinkString = "";
            if (hasThink)
            {
                if (isAllFair)
                {
                    if (isLiveEvent)
                    {
                        hasThinkString = " wl(think.i) -> Phil(i) []";
                    }
                    else
                    {
                        hasThinkString = " wf(think.i) -> Phil(i) []";
                    }
                }
                else
                {
                    hasThinkString = " think.i -> Phil(i) []";
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("//@@Dining Philosopher@@");
            sb.AppendLine("////////////////The Model//////////////////");
            sb.AppendLine("#define N " + N + ";");
            sb.AppendLine();

            if (isWL && isAllFair)
            {
                if (isLiveEvent)
                {
                    sb.AppendLine("Phil(i) =" + hasThinkString + " wl(get.i.(i+1)%N) -> wl(get.i.i) -> wl(eat.i) -> wl(put.i.(i+1)%N) -> wl(put.i.i) -> Phil(i);");
                    sb.AppendLine("Fork(x) = wl(get.x.x) -> wl(put.x.x) -> Fork(x) [] wl(get.(x-1)%N.x) -> wl(put.(x-1)%N.x) -> Fork(x);");
                }
                else
                {
                    sb.AppendLine("Phil(i) =" + hasThinkString + " wf(get.i.(i+1)%N) -> wf(get.i.i) -> wf(eat.i) -> wf(put.i.(i+1)%N) -> wf(put.i.i) -> Phil(i);");
                    sb.AppendLine("Fork(x) = wf(get.x.x) -> wf(put.x.x) -> Fork(x) [] wf(get.(x-1)%N.x) -> wf(put.(x-1)%N.x) -> Fork(x);");
                }
            }

            if (isWL && !isAllFair)
            {
                if (isLiveEvent)
                {
                    sb.AppendLine("Phil(i) =" + hasThinkString + " wl(get.i.(i+1)%N) -> get.i.i -> eat.i -> put.i.(i+1)%N -> put.i.i -> Phil(i);");
                    sb.AppendLine("Fork(x) = get.x.x -> wl(put.x.x) -> Fork(x) [] get.(x-1)%N.x -> wl(put.(x-1)%N.x) -> Fork(x);");
                }
                else
                {
                    sb.AppendLine("Phil(i) = " + hasThinkString + " wf(get.i.(i+1)%N) -> get.i.i -> eat.i -> put.i.(i+1)%N -> put.i.i -> Phil(i);");
                    sb.AppendLine("Fork(x) = get.x.x -> wf(put.x.x) -> Fork(x) [] get.(x-1)%N.x -> wf(put.(x-1)%N.x) -> Fork(x);");
                }
            }

            if (!isWL)
            {
                sb.AppendLine("Phil(i) =" + hasThinkString + " get.i.(i+1)%N -> get.i.i -> eat.i -> put.i.(i+1)%N -> put.i.i -> Phil(i);");
                sb.AppendLine("Fork(x) = get.x.x -> put.x.x -> Fork(x) [] get.(x-1)%N.x -> put.(x-1)%N.x -> Fork(x);");
            }

            sb.AppendLine("College() = ||x:{0..N-1}@(Phil(x)||Fork(x));");

            string tmp = "get.0.0,get.0.1,put.0.0,put.0.1";
            for (int i = 1; i < N; i++)
            {
                tmp += ",eat." + i;
                tmp += ",get." + i + "." + i;
                tmp += ",get." + i + "." + (i+1)%N;
                tmp += ",put." + i + "." + i;
                tmp += ",put." + i + "." + (i + 1) % N;
            }
            
            sb.AppendLine("Implementation() = College() \\ {" + tmp + "};");

            sb.AppendLine("Specification() = eat.0 -> Specification();");
            //sb.AppendLine();

            sb.AppendLine("////////////////The Properties//////////////////");
            sb.AppendLine("#assert College() deadlockfree;");
            sb.AppendLine("#assert College() |= []<> eat.0;");

            //tmp = "";
            //for (int i = 0; i < N; i++)
            //{
            //    tmp += "[]<>eat." + i + " && ";
            //}

            //sb.AppendLine("#assert College() |= " + tmp.Substring(0, tmp.Length - 4) + ";");
            sb.AppendLine("#assert Implementation() refines Specification();");
            sb.AppendLine("#assert Specification() refines Implementation();");
            sb.AppendLine("#assert Implementation() refines <F> Specification();");
            sb.AppendLine("#assert Specification() refines <F> Implementation();");
            sb.AppendLine("#assert Implementation() refines <FD> Specification();");
            sb.AppendLine("#assert Specification() refines <FD> Implementation();");

            return sb.ToString();
        }


        /// <summary>
        /// This example is the classic Dining Philosophiers. 
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public static string LoadDeadlockFreeDP(int N)
        {
            string hasThinkString = "";
       
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("////////////////The Model//////////////////");
            sb.AppendLine("#define N " + N + ";");
            sb.AppendLine();


            sb.AppendLine("Phil(i) = get.i.(i+1)%N -> get.i.i -> eat.i -> put.i.(i+1)%N -> put.i.i -> Phil(i);");
            sb.AppendLine("Fork(x) = get.x.x -> put.x.x -> Fork(x) [] get.(x-1)%N.x -> put.(x-1)%N.x -> Fork(x);");

            sb.AppendLine("Phil0 = get.0.0 -> get.0.1 -> eat.0 -> put.0.0 -> put.0.1 -> Phil0;");


            sb.AppendLine("College() = Phil0|| Fork(0)||(||x:{1..N-1}@(Phil(x)||Fork(x))) ; ");
            sb.AppendLine();

            sb.AppendLine("////////////////The Properties//////////////////");
            sb.AppendLine("#assert College() deadlockfree;");
            sb.AppendLine("#assert College() |= []<> eat.0;");
            return sb.ToString();
        }


        public static string LoadPetersonAlgo(int N, bool isFair)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("////////////////The Model//////////////////");
            sb.AppendLine("#define N " + N + ";");
            sb.AppendLine();

            sb.AppendLine("var step[N];");
            sb.AppendLine("var pos[N];");
            sb.AppendLine("var counter = 0; //which counts how many processes are in the critical session.");

            string temp = "";

            for (int i = 0; i < N; i++)
            {
                sb.AppendLine("Process" + i + "() = Repeat" + i + "(1); cs." + i + "{counter = counter+1;} -> reset{pos[" + i + "] = 0; counter = counter-1;} -> Process" + i + "();");
                if (!isFair)
                {
                    sb.AppendLine("Repeat" + i + "(j) = [j < N] update." + i + ".1{pos[" + i + "] = j;} -> update." + i + ".2{step[j] = " + i + ";} -> ");
                }
                else
                {
                    sb.AppendLine("Repeat" + i + "(j) = [j < N] wf(update." + i + ".1){pos[" + i + "] = j;} -> wf(update." + i + ".2){step[j] = " + i + ";} -> ");
                }
                temp = "";

                for (int j = 0; j < N; j++)
                {
                    if (i != j)
                    {
                        temp += "pos[" + j + "] < j && ";
                    }
                }
                temp = temp.Remove(temp.Length - 4, 4);
                sb.AppendLine("\t\t\t\t([step[j] != " + i + " || (" + temp + ")]idle.j -> Repeat" + i + "(j+1))");
                sb.AppendLine("\t\t[] [j == N] Skip;");
                sb.AppendLine();
            }

            temp = "Peterson() = ";
            for (int i = 0; i < N; i++)
            {
                temp += "Process" + i + "() ||| ";
            }

            temp = temp.Remove(temp.Length - 5, 5);
            sb.AppendLine(temp + ";");
            sb.AppendLine();

            sb.AppendLine("#define goal counter > 1;");
            sb.AppendLine("#assert Peterson() reaches goal;");
            sb.AppendLine("#assert Peterson() |= []<> cs.0;");

            return sb.ToString();
        }

        public static string LoadNeedhamSchroederProtocol(int N, bool isCorrect)
        {
            StringBuilder sb = new StringBuilder();

            if (isCorrect)
            {
                sb.AppendLine("// Fixed Version of Needham-Schroeder Public-Key Protocol");
                sb.AppendLine();

                sb.AppendLine("//A is Alice, the initiator;");
                sb.AppendLine("//B is Bob, the responder;");
                sb.AppendLine("//I is the Intruder;");
                sb.AppendLine("//Na is the Nonce A;");
                sb.AppendLine("//Nb is the Nonce B;");
                sb.AppendLine("//gD is the generic Data;");
                sb.AppendLine("enum {A, B, I, Na, Nb, gD};");
                sb.AppendLine();

                sb.AppendLine("//ca: type 1 messages {x1,x2}PK{x3}");
                sb.AppendLine("//cb: type 2 messages {x1}PK{x2}");
                sb.AppendLine("//cc: type 3 messages {x1,x2,x3}PK{x4}");
                sb.AppendLine("channel ca 0;");
                sb.AppendLine("channel cb 0;");
                sb.AppendLine("channel cc 0;");
                sb.AppendLine();
                sb.AppendLine("//IniRunningAB is true iff initiator A takes part in a session of the protocol with B.");
                sb.AppendLine("var IniRunningAB = false;");
                sb.AppendLine("//IniCommitAB is true iff initiator A commits to a session with B.");
                sb.AppendLine("var IniCommitAB = false;");
                sb.AppendLine("//ResRunningAB is true iff responder B takes part in a session of the protocol with A.");
                sb.AppendLine("var ResRunningAB = false;");
                sb.AppendLine("//ResCommitAB is true iff responder B commits to a session with A.");
                sb.AppendLine("var ResCommitAB = false;");
                sb.AppendLine();
                sb.AppendLine("//Initiator");
                sb.AppendLine("PIni(sender, receiver, nonce) =");
                sb.AppendLine("	IniRunning_AB { if (sender == A && receiver == B) { IniRunningAB = true; } } ->");
                sb.AppendLine("	");
                sb.AppendLine("	//sending {nonce, sender}Pk{receiver}");
                sb.AppendLine("	ca!sender.nonce.sender.receiver ->");
                sb.AppendLine("	");
                sb.AppendLine("	//receiving {nonce, g1}Pk{sender}");
                sb.AppendLine("	cc?sender.nonce.g1.receiver.sender -> IniCommit_AB { if (sender == A && receiver == B) { IniCommitAB = true; } } ->");
                sb.AppendLine("	");
                sb.AppendLine("	//sending {g1}Pk{receiver}");
                sb.AppendLine("	cb!sender.g1.receiver -> Skip;");
                sb.AppendLine();
                sb.AppendLine("//Responder");
                sb.AppendLine("PRes(receiver, nonce) =	");
                sb.AppendLine("	//receiving {g2, g3}Pk{receiver}");
                sb.AppendLine("	ca?receiver.g2.g3.receiver -> ResRunning_AB { if (g3 == A && receiver == B) { ResRunningAB = true; } } ->");
                sb.AppendLine("	");
                sb.AppendLine("	//sending {g2, nonce, receiver}Pk{g3}");
                sb.AppendLine("	cc!receiver.g2.nonce.receiver.g3 ->");
                sb.AppendLine("		");
                sb.AppendLine("	//receiving {nonce}Pk{receiver}");
                sb.AppendLine("	cb?receiver.nonce.receiver -> ResCommit_AB { if (g3 == A && receiver == B) { ResCommitAB = true; } } -> Skip;");
                sb.AppendLine();

                sb.AppendLine("//Intruder knows Na");
                sb.AppendLine("var kNa = false; ");
                sb.AppendLine("//Intruder knows Nb");
                sb.AppendLine("var kNb = false;");
                sb.AppendLine("//Intruder knows {Na, Nb, B}PK{A}");
                sb.AppendLine("var k_Na_Nb_B__A = false;");
                sb.AppendLine("//Intruder knows {Na, A}PK{B}");
                sb.AppendLine("var k_Na_A__B = false;");
                sb.AppendLine("//Intruder knows {Nb}PK{B}");
                sb.AppendLine("var k_Nb__B = false;");

                sb.AppendLine();
                sb.AppendLine("//Intruder Process, which always knows A, B, I, PK(A), PK(B), PK(I), SK(I) and Ng");
                sb.AppendLine("PI() =");
                sb.AppendLine("	ca!B.gD.A.B -> PI() []");
                sb.AppendLine("	ca!B.gD.B.B -> PI() []");
                sb.AppendLine("	ca!B.gD.I.B -> PI() []");
                sb.AppendLine("	");
                sb.AppendLine("	ca!B.A.A.B -> PI() []");
                sb.AppendLine("	ca!B.A.B.B -> PI() []");
                sb.AppendLine("	ca!B.A.I.B -> PI() []");
                sb.AppendLine("	");
                sb.AppendLine("	ca!B.B.A.B -> PI() []");
                sb.AppendLine("	ca!B.B.B.B -> PI() []");
                sb.AppendLine("	ca!B.B.I.B -> PI() []");
                sb.AppendLine("	");
                sb.AppendLine("	ca!B.I.A.B -> PI() []");
                sb.AppendLine("	ca!B.I.B.B -> PI() []");
                sb.AppendLine("	ca!B.I.I.B -> PI() []");
                sb.AppendLine("	");
                sb.AppendLine("	[kNa]                        cc!A.Na.Na.B.A -> PI() []");
                sb.AppendLine("	[(kNa && kNb) || k_Na_Nb_B__A] cc!A.Na.Nb.B.A -> PI() []");
                sb.AppendLine("	[kNa]                        cc!A.Na.gD.B.A -> PI() []");
                sb.AppendLine("	");
                sb.AppendLine("	[kNa] cc!A.Na.A.B.A -> PI() []");
                sb.AppendLine("	[kNa] cc!A.Na.B.B.A -> PI() []");
                sb.AppendLine("	[kNa] cc!A.Na.I.B.A -> PI() []");
                sb.AppendLine("	");
                sb.AppendLine("	[kNa]        cc!A.Na.Na.I.A -> PI() []");
                sb.AppendLine("	[kNa && kNb] cc!A.Na.Nb.I.A -> PI() []");
                sb.AppendLine("	[kNa]        cc!A.Na.gD.I.A -> PI() []");
                sb.AppendLine("	");
                sb.AppendLine("	[kNa] cc!A.Na.A.I.A -> PI() []");
                sb.AppendLine("	[kNa] cc!A.Na.B.I.A -> PI() []");
                sb.AppendLine("	[kNa] cc!A.Na.I.I.A -> PI() []");
                sb.AppendLine("	");
                sb.AppendLine("	[kNa || k_Na_A__B] ca!B.Na.A.B -> PI() []");
                sb.AppendLine("	[kNa]              ca!B.Na.B.B -> PI() []");
                sb.AppendLine("	[kNa]              ca!B.Na.I.B -> PI() []");
                sb.AppendLine("	");
                sb.AppendLine("	[kNb] ca!B.Nb.A.B -> PI() []");
                sb.AppendLine("	[kNb] ca!B.Nb.B.B -> PI() []");
                sb.AppendLine("	[kNb] ca!B.Nb.I.B -> PI() []");
                sb.AppendLine("	");
                sb.AppendLine("	[k_Nb__B || kNb] cb!B.Nb.B -> PI() []");
                sb.AppendLine("	");
                sb.AppendLine("	ca?tmp1.x1.x2.x3");
                sb.AppendLine("	-> InterceptChanA {");
                sb.AppendLine("		if (x3 == I) {");
                sb.AppendLine("			if (x1 == Na) { kNa = true; }");
                sb.AppendLine("			else if (x1 == Nb) { kNb = true; }");
                sb.AppendLine("			if (x2 == Na) { kNa = true; }");
                sb.AppendLine("			else if (x2 == Nb) { kNb = true; }");
                sb.AppendLine("		}");
                sb.AppendLine("		else if (x1 == Na && x2 == A && x3 == B) { k_Na_A__B = true; }");
                sb.AppendLine("	} -> PI() []");
                sb.AppendLine("	");
                sb.AppendLine("	cb?tmp2.y1.y2");
                sb.AppendLine("	-> InterceptChanB {");
                sb.AppendLine("		if (y2 == I) {");
                sb.AppendLine("			if (y1 == Na) { kNa = true; }");
                sb.AppendLine("			else if (y1 == Nb) { kNb = true; }");
                sb.AppendLine("		}");
                sb.AppendLine("		else if (y1 == Nb && y2 == B) { k_Nb__B = true; }");
                sb.AppendLine("	}");
                sb.AppendLine("	->  PI() []");
                sb.AppendLine("	");
                sb.AppendLine("	cc?tmp3.z1.z2.z3.z4 ");
                sb.AppendLine("	-> InterceptChanC {");
                sb.AppendLine("		if (z4 == I) {");
                sb.AppendLine("			if (z1 == Na) { kNa = true; }");
                sb.AppendLine("			else if (z1 == Nb) { kNb = true; }");
                sb.AppendLine("			if (z2 == Na) { kNa = true; }");
                sb.AppendLine("			else if (z2 == Nb) { kNb = true; }");
                sb.AppendLine("			if (z3 == Na) { kNa = true; }");
                sb.AppendLine("			else if (z3 == Nb) { kNb = true; }");
                sb.AppendLine("		}");
                sb.AppendLine("		else if (z1 == Na && z2 == Nb && z3 == B && z4 == A) { k_Na_Nb_B__A = true; }");
                sb.AppendLine("	} -> PI;");

                sb.AppendLine();
                sb.AppendLine("Protocol = ( PIni(A, I, Na) [] PIni(A, B, Na) ) ||| PRes(B, Nb) ||| PI;");
                sb.AppendLine();
                sb.AppendLine("#define iniRunningAB (IniRunningAB == true);");
                sb.AppendLine("#define iniCommitAB (IniCommitAB == true);");
                sb.AppendLine("#define resRunningAB (ResRunningAB == true);");
                sb.AppendLine("#define resCommitAB (ResCommitAB == true);");
                sb.AppendLine();
                sb.AppendLine("//Authentication of B to A can thus be expressed saying that ResRunningAB must become true before IniCommitAB.");
                sb.AppendLine("//i.e., the initiator A commits to a session with B only if B has indeed taken part in a run of the protocol with A.");
                sb.AppendLine("#assert Protocol |= [] ( ([] !iniCommitAB) || (!iniCommitAB U resRunningAB) );");
                sb.AppendLine();
                sb.AppendLine("//The converse authentication property corresponds to saying that IniRunningAB becomes true before ResCommitAB.");
                sb.AppendLine("//The flaw of the protocol is shown by this model");
                sb.AppendLine("#assert Protocol |= [] ( ([] !resCommitAB) || (!resCommitAB U iniRunningAB) );");
                sb.AppendLine();
                sb.AppendLine("#assert Protocol deadlockfree;");


            }
            else
            {
                sb.AppendLine("// Original Version of Needham-Schroeder Public-Key Protocol");
                sb.AppendLine();

                sb.AppendLine("//A is Alice, the initiator;");
                sb.AppendLine("//B is Bob, the responder;");
                sb.AppendLine("//I is the Intruder;");
                sb.AppendLine("//Na is the Nonce A;");
                sb.AppendLine("//Nb is the Nonce B;");
                sb.AppendLine("//gD is the generic Data;");
                sb.AppendLine("enum {A, B, I, Na, Nb, gD};");
                sb.AppendLine();

                sb.AppendLine("//ca: type 1 messages {x1,x2}PK{x3}");
                sb.AppendLine("//cb: type 2 messages {x1}PK{x2}");
                sb.AppendLine("channel ca 0;");
                sb.AppendLine("channel cb 0;");
                sb.AppendLine();

                sb.AppendLine("//IniRunningAB is true iff initiator A takes part in a session of the protocol with B.");
                sb.AppendLine("var IniRunningAB = false;");
                sb.AppendLine("//IniCommitAB is true iff initiator A commits to a session with B.");
                sb.AppendLine("var IniCommitAB = false;");
                sb.AppendLine("//ResRunningAB is true iff responder B takes part in a session of the protocol with A.");
                sb.AppendLine("var ResRunningAB = false;");
                sb.AppendLine("//ResCommitAB is true iff responder B commits to a session with A.");
                sb.AppendLine("var ResCommitAB = false;");
                sb.AppendLine();

                sb.AppendLine("//Initiator");
                sb.AppendLine("PIni(sender, receiver, nonce) =");
                sb.AppendLine("	IniRunning_AB { if (sender == A && receiver == B) { IniRunningAB = true; } } ->");
                sb.AppendLine("	//sending {nonce, sender}Pk{receiver}");
                sb.AppendLine("	ca!sender.nonce.sender.receiver ->");
                sb.AppendLine("	");
                sb.AppendLine("	//receiving {nonce, g1}Pk{sender}");
                sb.AppendLine("	ca?sender.nonce.g1.sender -> IniCommit_AB { if (sender == A && receiver == B) { IniCommitAB = true; } } ->");
                sb.AppendLine("	");
                sb.AppendLine("	//sending {g1}Pk{receiver}");
                sb.AppendLine("	cb!sender.g1.receiver -> Skip;");
                sb.AppendLine();

                sb.AppendLine("//Responder");
                sb.AppendLine("PRes(receiver, nonce) =	");
                sb.AppendLine("	//receiving {g2, g3}Pk{receiver}");
                sb.AppendLine("	ca?receiver.g2.g3.receiver ->");
                sb.AppendLine("	ResRunning_AB { if (g3 == A && receiver == B) { ResRunningAB = true; } } ->");
                sb.AppendLine("	");
                sb.AppendLine("	//sending {g2, nonce}Pk{g3}");
                sb.AppendLine("	ca!receiver.g2.nonce.g3 ->");
                sb.AppendLine("	");
                sb.AppendLine("	//receiving {nonce}Pk{receiver}");
                sb.AppendLine("	cb?receiver.nonce.receiver ->");
                sb.AppendLine("	ResCommit_AB { if (g3 == A && receiver == B) { ResCommitAB = true; } } -> Skip;");
                sb.AppendLine();

                sb.AppendLine("//Intruder knows Na");
                sb.AppendLine("var kNa = false; ");
                sb.AppendLine("//Intruder knows Nb");
                sb.AppendLine("var kNb = false;");
                sb.AppendLine("//Intruder knows {Na, Nb}PK{A}");
                sb.AppendLine("var k_Na_Nb__A = false;");
                sb.AppendLine("//Intruder knows {Na, A}PK{B}");
                sb.AppendLine("var k_Na_A__B = false;");
                sb.AppendLine("//Intruder knows {Nb}PK{B}");
                sb.AppendLine("var k_Nb__B = false;");
                sb.AppendLine();

                sb.AppendLine("//Intruder Process, which always knows A, B, I, PK(A), PK(B), PK(I), SK(I) and Ng");
                sb.AppendLine("PI() =");
                sb.AppendLine("	ca!B.gD.A.B -> PI() []");
                sb.AppendLine("	ca!B.gD.B.B -> PI() []");
                sb.AppendLine("	ca!B.gD.I.B -> PI() []");
                sb.AppendLine("	");
                sb.AppendLine("	ca!B.A.A.B -> PI []");
                sb.AppendLine("	ca!B.A.B.B -> PI []");
                sb.AppendLine("	ca!B.A.I.B -> PI []");
                sb.AppendLine("	");
                sb.AppendLine("	ca!B.B.A.B -> PI() []");
                sb.AppendLine("	ca!B.B.B.B -> PI() []");
                sb.AppendLine("	ca!B.B.I.B -> PI() []");
                sb.AppendLine("	");
                sb.AppendLine("	ca!B.I.A.B -> PI() []");
                sb.AppendLine("	ca!B.I.B.B -> PI() []");
                sb.AppendLine("	ca!B.I.I.B -> PI() []");
                sb.AppendLine("	");
                sb.AppendLine("	[kNa]                        ca!A.Na.Na.A -> PI() []");
                sb.AppendLine("	[(kNa && kNb) || k_Na_Nb__A] ca!A.Na.Nb.A -> PI() []");
                sb.AppendLine("	[kNa]                        ca!A.Na.gD.A -> PI() []");
                sb.AppendLine("	");
                sb.AppendLine("	[kNa] ca!A.Na.A.A -> PI() []");
                sb.AppendLine("	[kNa] ca!A.Na.B.A -> PI() []");
                sb.AppendLine("	[kNa] ca!A.Na.I.A -> PI() []");
                sb.AppendLine("	");
                sb.AppendLine("	[kNa || k_Na_A__B] ca!B.Na.A.B -> PI() []");
                sb.AppendLine("	[kNa]              ca!B.Na.B.B -> PI() []");
                sb.AppendLine("	[kNa]              ca!B.Na.I.B -> PI() []");
                sb.AppendLine("	");
                sb.AppendLine("	[kNb] ca!B.Nb.A.B -> PI() []");
                sb.AppendLine("	[kNb] ca!B.Nb.B.B -> PI() []");
                sb.AppendLine("	[kNb] ca!B.Nb.I.B -> PI() []");
                sb.AppendLine("	");
                sb.AppendLine("	[k_Nb__B || kNb] cb!B.Nb.B -> PI() []");
                sb.AppendLine("	");
                sb.AppendLine("	ca?tmp1.x1.x2.x3");
                sb.AppendLine("	-> InterceptChanA {");
                sb.AppendLine("		if (x3 == I) {");
                sb.AppendLine("			if (x1 == Na) { kNa = true; }");
                sb.AppendLine("			else if (x1 == Nb) { kNb = true; }");
                sb.AppendLine("			if (x2 == Na) { kNa = true; }");
                sb.AppendLine("			else if (x2 == Nb) { kNb = true; }");
                sb.AppendLine("		}");
                sb.AppendLine("		else if (x1 == Na && x2 == A && x3 == B) { k_Na_A__B = true; }");
                sb.AppendLine("		else if (x1 == Na && x2 == Nb && x3 == A) { k_Na_Nb__A = true; }");
                sb.AppendLine("	}");
                sb.AppendLine("	-> PI() []");
                sb.AppendLine("	");
                sb.AppendLine("	cb?tmp2.y1.y2");
                sb.AppendLine("	-> InterceptChanB {");
                sb.AppendLine("		if (y2 == I) {");
                sb.AppendLine("			if (y1 == Na) { kNa = true; }");
                sb.AppendLine("			else if (y1 == Nb) { kNb = true; }");
                sb.AppendLine("		}");
                sb.AppendLine("		else if (y1 == Nb && y2 == B) { k_Nb__B = true; }");
                sb.AppendLine("	}");
                sb.AppendLine("	-> PI();");
                sb.AppendLine();

                sb.AppendLine("Protocol = ( PIni(A, I, Na) [] PIni(A, B, Na) ) ||| PRes(B, Nb) ||| PI;");
                sb.AppendLine();

                sb.AppendLine("#define iniRunningAB (IniRunningAB == true);");
                sb.AppendLine("#define iniCommitAB (IniCommitAB == true);");
                sb.AppendLine("#define resRunningAB (ResRunningAB == true);");
                sb.AppendLine("#define resCommitAB (ResCommitAB == true);");
                sb.AppendLine();

                sb.AppendLine("//Authentication of B to A can thus be expressed saying that ResRunningAB must become true before IniCommitAB.");
                sb.AppendLine("//i.e., the initiator A commits to a session with B only if B has indeed taken part in a run of the protocol with A.");
                sb.AppendLine("#assert Protocol |= [] ( ([] !iniCommitAB) || (!iniCommitAB U resRunningAB) );");
                sb.AppendLine();

                sb.AppendLine("//The converse authentication property corresponds to saying that IniRunningAB becomes true before ResCommitAB.");
                sb.AppendLine("//The flaw of the protocol is shown by this model");
                sb.AppendLine("#assert Protocol |= [] ( ([] !resCommitAB) || (!resCommitAB U iniRunningAB) );");
                sb.AppendLine();

                sb.AppendLine("#assert Protocol deadlockfree;");



            }
 
            return sb.ToString();
        }

        public static string LoadInterruptController(int N)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// This model is based on the example of Section 2 in");
            sb.AppendLine("// Priorities in Process Algebras, by Rance Cleveland and");
            sb.AppendLine("// Matthew Hennesy (Information and Computation, 1990).");
            sb.AppendLine("");
            sb.AppendLine("// The interrupt controller INT should communicate 'i'");
            sb.AppendLine("// whenever possible, so that C cannot receive 'up' or 'down'");
            sb.AppendLine("// after INT has received 'shut_down'.");
            sb.AppendLine("");
            sb.AppendLine("// Uncomment one of the priority specifications to make the model work,");
            sb.AppendLine("// by using channel priorities (end of these declarations), or");
            sb.AppendLine("// process priorities (system declaration).");
            sb.AppendLine("");
            sb.AppendLine("// Counter modulo (wrap-around).");
            sb.AppendLine("#define MODULO " + N + ";");
            sb.AppendLine("");
            sb.AppendLine("// Input to the counter");
            sb.AppendLine("channel up 0;");
            sb.AppendLine("channel down 0;");
            sb.AppendLine("");
            sb.AppendLine("// Shut down request");
            sb.AppendLine("channel shutdown 0;");
            sb.AppendLine("");
            sb.AppendLine("// Internal communication");
            sb.AppendLine("channel i 0;");
            sb.AppendLine("");
            sb.AppendLine("var noOfError = 0;");
            sb.AppendLine("");
            sb.AppendLine("var count;");
            sb.AppendLine("");
            sb.AppendLine("Int = shutdown?0 -> i!0 -> Stop;");
            sb.AppendLine("");
            sb.AppendLine("C = atomic{up?0 -> tau{count = (count + 1) % MODULO; } -> Skip}; C [] atomic{down?0 -> tau{if(count > 0) {count = count - 1;}} -> Skip}; C [] i?0 -> Stop;");
            sb.AppendLine("");
            sb.AppendLine("Evn = up!0 -> Evn [] down!0 -> Evn []  shutdown!0 -> ( up!0 -> Error() [] down!0 -> Error());");
            sb.AppendLine("");
            sb.AppendLine("Error = error{noOfError++} -> Stop;");
            sb.AppendLine("");
            sb.AppendLine("aSys = Int ||| C ||| Evn;");
            sb.AppendLine("");
            sb.AppendLine("#define goal noOfError > 0;");
            sb.AppendLine("#assert aSys reaches goal;");
            return sb.ToString();
        }

        public static string LoadOrientingUndirectedInRing(int node)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("////////////////  the number of nodes, the number of colors (d*(d-1)+1), d is the degree of the graph");
            sb.AppendLine("//////////////// we only focus on the ring topology, hence, C=3");
            sb.AppendLine("#define N "+node+";");
            sb.AppendLine("#define C 3;");
            sb.AppendLine();

            sb.AppendLine("////////////////Global Variables//////////////////");
            sb.AppendLine("var mycolor[N];");
            sb.AppendLine("var precolor[N];");
            sb.AppendLine("var succolor[N];");
            sb.AppendLine();

            sb.AppendLine("//////////////// Interaction //////////////////");
            sb.AppendLine("Interaction(u,v) =    ");
            sb.AppendLine("	if (mycolor[v]==precolor[u] && mycolor[v]!=succolor[u]) { ");
            sb.AppendLine("		interact.u.v{succolor[v] = mycolor[u];} -> Interaction(u,v)");
            sb.AppendLine("        } else { ");
            sb.AppendLine("		if (mycolor[v]==succolor[u] && mycolor[v]!=precolor[u]) { ");
            sb.AppendLine("			interact.u.v{precolor[v] = mycolor[u];} -> Interaction(u,v) ");
            sb.AppendLine("		} else{ ");
            sb.AppendLine("			interact.u.v{precolor[u] = mycolor[v]; succolor[v] = mycolor[u];} -> Interaction(u,v)");
            sb.AppendLine("		}");
            sb.AppendLine("	};");
            sb.AppendLine();

            sb.AppendLine("/////////////// Inputs should be two-hop coloring on the nodes,");
            sb.AppendLine("/////////////// which means the neighbors of one node must have different colors.");
            sb.AppendLine();

            sb.AppendLine("/////////////// Since the ring has nodes with 3 colors, the only way is to make nodes with unique color.");
            sb.AppendLine("/////////////// Cases are symmetric. Hence, we only give one possibility here.");
            sb.AppendLine();


            sb.AppendLine("Initialization() = (");

            if(node == 3)
            {
                sb.AppendLine("                     (t{mycolor[0] = 0; mycolor[1] = 1; mycolor[2] = 2;} -> Skip)");
            }
            else if (node == 4)
            {
                sb.AppendLine("                     (tau{mycolor[0] = 0; mycolor[1] = 0; mycolor[2] = 1; mycolor[3] = 2;} -> Skip)");
                sb.AppendLine("                     [](tau{mycolor[0] = 0; mycolor[1] = 0; mycolor[2] = 1; mycolor[3] = 1;} -> Skip)");
            }
            else if (node == 5)
            {
                sb.AppendLine("                     (tau{mycolor[0] = 0; mycolor[1] = 0; mycolor[2] = 2; mycolor[3] = 1; mycolor[4] = 1} -> Skip)");
                sb.AppendLine("                     [](tau{mycolor[0] = 0; mycolor[1] = 0; mycolor[2] = 2; mycolor[3] = 2; mycolor[4] = 1} -> Skip)");
                sb.AppendLine("                     [](tau{mycolor[0] = 0; mycolor[1] = 1; mycolor[2] = 1; mycolor[3] = 2; mycolor[4] = 2} -> Skip)");
            }
            else if (node == 6)
            {
                sb.AppendLine("                     (tau{mycolor[0] = 0; mycolor[1] = 0; mycolor[2] = 1; mycolor[3] = 2; mycolor[4] = 2; mycolor[5] =1;} -> Skip)");
                sb.AppendLine("                     [](tau{mycolor[0] = 0; mycolor[1] = 0; mycolor[2] = 2; mycolor[3] = 2; mycolor[4] = 1; mycolor[5] =1;} -> Skip)");
                sb.AppendLine("                     [](tau{mycolor[0] = 0; mycolor[1] = 1; mycolor[2] = 1; mycolor[3] = 0; mycolor[4] = 2; mycolor[5] =2;} -> Skip)");
                sb.AppendLine("                     [](tau{mycolor[0] = 0; mycolor[1] = 1; mycolor[2] = 2; mycolor[3] = 0; mycolor[4] = 1; mycolor[5] =2;} -> Skip)");
            }

            //for (int i = 3; i <= node; i++)

            //{
            //    if(i != 3)
            //    {
            //        sb.Append("                     []");                        
            //    }
            //    else
            //    {
            //        sb.Append("                     ");                        
            //    }

            //    sb.Append("(t{");

            //    for (int j = 0; j < node; j++)
            //    {
            //        sb.Append("mycolor["+j+"] = 0;");
            //    }

            //    sb.AppendLine("} -> Skip)");                        
            //}


            sb.AppendLine("                     );");

            for (int i = 0; i < node; i++)
            {
                sb.AppendLine("                     ((t{precolor[" + i + "] = 0;} -> Skip) [] (t{precolor[" + i + "] = 1;} -> Skip) [] (t{precolor[" + i + "] = 2;} -> Skip));");
            }

            for (int i = 0; i < node; i++)
            {
                sb.AppendLine("                     ((t{succolor[" + i + "] = 0;} -> Skip) [] (t{succolor[" + i + "] = 1;} -> Skip) [] (t{succolor[" + i + "] = 2;} -> Skip));");
            }
            sb.AppendLine();

            sb.AppendLine("/////// here, the interactions also define a ring");

            sb.AppendLine("OrientingUndirectedRing() = Initialization(); (");
            for (int i = 0; i < node; i++)
            {
                if (i == node - 1)
                {
                    sb.AppendLine("                     Interaction(" + i + "," + (i + 1) % node + ") ||| Interaction(" + i + "," + (i - 1) % node + ")");
                    
                }
                else if (i == 0)
                {
                    sb.AppendLine("                     Interaction(" + i + "," + (i + 1) % node + ") ||| Interaction(" + i + "," + (node - 1) + ") ||| ");
                }
                else 
                {
                    sb.AppendLine("                     Interaction(" + i + "," + (i + 1) % node + ") ||| Interaction(" + i + "," + (i - 1) % node + ") ||| ");
                }

            }

            sb.AppendLine("                     );");
            sb.AppendLine();

            sb.AppendLine("//////////////// The Properties //////////////////");
            sb.Append("#define presucdistinct (");
            for (int i = 0; i < node; i++)
            {
                if(i != node -1)
                {
                    sb.Append("(precolor[" + i + "]!=succolor[" + i + "]) &&");
                }
                else
                {
                    sb.Append("(precolor[" + i + "]!=succolor[" + i + "])");    
                }                
            }
            sb.AppendLine("                     );");

            string ltl = "";
            
            for (int i = 0; i < node; i++)
            {
                int i1 = (i + 1)%node;
                sb.AppendLine("#define adjecent" + i + i1 + " ((mycolor[" + i + "]==precolor[" + i1 + "] && mycolor[" + i1 + "]==succolor[" + i + "]) || (mycolor[" + i + "]==succolor[" + i1 + "] && mycolor[" + i1 + "]==precolor[" + i + "]));");

                if (i != node - 1)
                {
                    ltl += "adjecent" + i + i1 + " && ";
                }
                else
                {
                    ltl += "adjecent" + i + i1 + ";";
                }
            }
            sb.AppendLine();

            sb.AppendLine("#assert OrientingUndirectedRing() |= <>[] presucdistinct;");
            sb.AppendLine("#assert OrientingUndirectedRing() |= <>[] " + ltl);

            return sb.ToString();

        }


        public static string LoadConsensuswithCrashes(int node)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("");
            sb.AppendLine("//////////////// The Model //////////////////");
            sb.AppendLine("#define N "+node+";");
            sb.AppendLine("");
            sb.AppendLine("//////////////// Global Variables //////////////////");
            sb.AppendLine("var state[N];");
            sb.AppendLine("var crashed[N];");
            sb.AppendLine("");
            sb.AppendLine("var hascrashed=0;");
            sb.AppendLine("");

            sb.AppendLine("//////////////// Interaction //////////////////");
            sb.AppendLine("Interaction(i, r) =");
            sb.AppendLine("	    [ crashed[i]==0 && crashed[r]==0 && state[i] < state[r] ] (interaction.i.r{state[r] = state[i];} -> Interaction(i,r))");
            sb.AppendLine("	[]  [ crashed[i]==0 && hascrashed < N-1 ] (crash.i{crashed[i] = 1; hascrashed = hascrashed+1;} -> Skip);");
            sb.AppendLine("");
            sb.AppendLine("Initialization() = ");
             for (int i = 0; i < node; i++)
             {
                 sb.AppendLine("	((t{state[" +i+"] = 0; crashed[" +i+"] = 0;} -> Skip) [] (t{state[" +i+"] = 1; crashed[" +i+"] = 0;} -> Skip));");

             }
            sb.AppendLine("");

            sb.AppendLine("Consensus() = Initialization();(");
            
                for (int i = 0; i < node; i++)
                {
                    sb.Append("                  ");
                    for (int j = 0; j < node; j++)
                    {
                        if(i == j)
                        {
                            continue;
                        }

                        if(i == node - 1 && j == node -2)
                        {
                            sb.Append("Interaction("+i+","+j+"));");
                        }
                        else
                        {
                            sb.Append("Interaction("+i+","+j+") |||");
                        }

                    }
                    sb.AppendLine();
                }

  
            sb.AppendLine("");
            sb.AppendLine("//////////////// The Properties //////////////////");
            string ltl = "";
            
            for (int i = 0; i < node; i++)
            {
                  for (int j = i +1; j < node; j++)
                  {
                      sb.AppendLine("#define stabagree" + i + j + " (crashed[" + i + "]==0 && crashed[" + j + "]==0 && state[" + i + "]==state[" + j + "]);");

                      if (i != node - 2)
                      {
                          ltl += "stabagree" + i + j + " && ";
                      }
                      else
                      {
                          ltl += "stabagree" + i + j + ";";
                      }
                  }
            }
            sb.AppendLine();
            sb.AppendLine("#assert Consensus() |= <>[] " + ltl);
              for (int i = 0; i < node; i++)
              {
                  for (int j = i + 1; j < node; j++)
                  {
                      sb.AppendLine("#assert Consensus() |= <>[] stabagree" + i + j + ";");
                  }
              }

            return sb.ToString();
        }

        public static string LoadTwoHopColoringInRingsDeterministic(int node)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("////////////////  Deterministic 2-hop coloring in rings");

            sb.AppendLine("////////////////  the number of nodes N, the number of colors (d*(d-1)+1), d is the degree of the graph");
            sb.AppendLine("//////////////// we only focus on the ring topology, hence, C=3");
            sb.AppendLine("#define N "+node+";");

            sb.AppendLine("#define C 3;");

            sb.AppendLine("////////////////Global Variables//////////////////");
            sb.AppendLine("//////////////// color is a array of possible colors (C)");
            sb.AppendLine("//////////////// F is a bit array");
            sb.AppendLine("var color[N];");
            sb.AppendLine("var r[N];");
            sb.AppendLine("var F[N*C];");

            sb.AppendLine("//////////////// Interaction //////////////////");
            sb.AppendLine("Interaction(u,v) =    ");
            sb.AppendLine("	if (F[C*u+color[v]] != F[C*v+color[u]]) { ");
            sb.AppendLine("	     interact.u.v{ color[u] = (color[u] + r[u]) % C ; F[C*u+color[v]] = F[C*v+color[u]]; r[u] = 1-r[u]; } -> Interaction(u,v)");
            sb.AppendLine("        } else { ");
            sb.AppendLine("             interact.u.v{ F[C*u+color[v]] = 1- F[C*u+color[v]]; F[C*v+color[u]] = 1- F[C*v+color[u]];  r[u] = 1-r[u]; } -> Interaction(u,v)");
            sb.AppendLine("	};");

            sb.AppendLine("//////////////// Initialization //////////////////");
            sb.AppendLine("Initialization() = ");
            for (int i = 0; i < node; i++)
            {
                sb.AppendLine("	  ((t{color[" + i + "] = 0;}-> Skip) [] (t{color[" + i + "] = 1;}-> Skip) [] (tau{color[" + i + "] = 2;}-> Skip));");
            }

            for (int i = 0; i < node; i++)
            {
                sb.AppendLine("	  ((t{r[" + i + "] = 0;} -> Skip) [] (t{r[" + i + "] = 1;} -> Skip));");
            }

            for (int i = 0; i < node*3; i++)
            {
                sb.AppendLine("	  ((t{F[" + i + "] = 0;} -> Skip) [] (t{F[" + i + "] = 1;} -> Skip));");
            }

            sb.AppendLine("");

            sb.AppendLine("TwoHopColoringRings() = Initialization();(");
            for (int i = 0; i < node; i++)
            {
                if (i == node - 1)
                {
                    sb.AppendLine("	  Interaction(" + i + "," + (i + 1) % node + ") ||| Interaction(" + i + "," + (i - 1) % node + "));");

                }
                else if (i == 0)
                {
                    sb.AppendLine("	  Interaction(" + i + "," + (i + 1) % node + ") ||| Interaction(" + i + "," + (node - 1) + ") ||| ");
                }
                else
                {
                    sb.AppendLine("	  Interaction(" + i + "," + (i + 1) % node + ") ||| Interaction(" + i + "," + (i - 1) % node + ") ||| ");
                }

            }

            sb.AppendLine("");
            sb.AppendLine("//////////////// The Properties //////////////////");
            sb.Append("#define twohopcoloring ");

            for (int i = 0; i < node; i++)
            {
                if(i != node -1)
                {
                    sb.Append("(color[" + i + "]!=color[" + (i + 2)%node + "]) && ");    
                }
                else
                {
                    sb.Append("(color[" + i + "]!=color[" + (i + 2) % node + "])");    
                }                
            }

            sb.AppendLine(";");
            sb.AppendLine();

            sb.AppendLine("#assert TwoHopColoringRings() |= <>[] twohopcoloring;");


            return sb.ToString();

        }

        public static string LoadTwoHopColoringInRingsNonDeterministic(int node)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("////////////////  Nondeterministic 2-hop coloring in rings");

            sb.AppendLine("////////////////  the number of nodes N, the number of colors (d*(d-1)+1), d is the degree of the graph");
            sb.AppendLine("//////////////// we only focus on the ring topology, hence, C=3");
            sb.AppendLine("#define N "+node+";");

            sb.AppendLine("#define C 3;");

            sb.AppendLine("////////////////Global Variables//////////////////");
            sb.AppendLine("//////////////// color is a array of possible colors (C)");
            sb.AppendLine("//////////////// F is a bit array");
            sb.AppendLine("var color[N];");
            sb.AppendLine("var F[N*C];");

            sb.AppendLine("//////////////// Interaction //////////////////");
            sb.AppendLine("Interaction(u,v) =");
            sb.AppendLine("	if ( F[C*u+color[v]] != F[C*v+color[u]]) { ");
            sb.AppendLine("	     interact.u.v{ color[u] = 0; F[C*u+color[v]] = F[C*v+color[u]]; } -> Interaction(u,v)");
            sb.AppendLine("	  [] interact.u.v{ color[u] = 1; F[C*u+color[v]] = F[C*v+color[u]]; } -> Interaction(u,v)");
            sb.AppendLine("	  [] interact.u.v{ color[u] = 2; F[C*u+color[v]] = F[C*v+color[u]]; } -> Interaction(u,v)");
            sb.AppendLine("        } else { ");
            sb.AppendLine("             interact.u.v{ F[C*u+color[v]] = 1- F[C*u+color[v]]; F[C*v+color[u]] = 1- F[C*v+color[u]]; } -> Interaction(u,v)");
            sb.AppendLine("	};");

            sb.AppendLine("//////////////// Initialization //////////////////");
            sb.AppendLine("Initialization() = ");
            for (int i = 0; i < node; i++)
            {
                sb.AppendLine("	  ((t{color[" + i + "] = 0;}-> Skip) [] (t{color[" + i + "] = 1;}-> Skip) [] (tau{color[" + i + "] = 2;}-> Skip));");
            }

            for (int i = 0; i < node * 3; i++)
            {
                sb.AppendLine("	  ((t{F[" + i + "] = 0;} -> Skip) [] (t{F[" + i + "] = 1;} -> Skip));");
            }

            sb.AppendLine("");

            sb.AppendLine("TwoHopColoringRings() = Initialization();(");
            for (int i = 0; i < node; i++)
            {
                if (i == node - 1)
                {
                    sb.AppendLine("	  Interaction(" + i + "," + (i + 1) % node + ") ||| Interaction(" + i + "," + (i - 1) % node + "));");

                }
                else if (i == 0)
                {
                    sb.AppendLine("	  Interaction(" + i + "," + (i + 1) % node + ") ||| Interaction(" + i + "," + (node - 1) + ") ||| ");
                }
                else
                {
                    sb.AppendLine("	  Interaction(" + i + "," + (i + 1) % node + ") ||| Interaction(" + i + "," + (i - 1) % node + ") ||| ");
                }

            }

            sb.AppendLine("");
            sb.AppendLine("//////////////// The Properties //////////////////");
            sb.Append("#define twohopcoloring ");

            for (int i = 0; i < node; i++)
            {
                if (i != node - 1)
                {
                    sb.Append("(color[" + i + "]!=color[" + (i + 2) % node + "]) && ");
                }
                else
                {
                    sb.Append("(color[" + i + "]!=color[" + (i + 2) % node + "])");
                }
            }

            sb.AppendLine(";");
            sb.AppendLine();

            sb.AppendLine("#assert TwoHopColoringRings() |= <>[] twohopcoloring;");


            return sb.ToString();

        }

        public static string LoadLeaderElectionRing(int N)
        {
            string temp = "((detectorcorrect==0 && detector) || (detectorcorrect!=0 && ";
            for (int i = 0; i < N; i++)
            {
                temp += "leader[" + i + "]+";
            }

            temp = temp.Remove(temp.Length - 1, 1);
            temp += " > 0))";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("////////////////The Model//////////////////");
            sb.AppendLine("#define N " + N + ";");
            sb.AppendLine();

            sb.AppendLine("////////////////Global Variables//////////////////");
            sb.AppendLine("var detectorcorrect = 0;");
            sb.AppendLine("var detector = false;");
            sb.AppendLine("var leader[N];");
            sb.AppendLine("var bullet[N];");
            sb.AppendLine("var shield[N];");
            sb.AppendLine();

            sb.AppendLine("////////////////Processes//////////////////");
            sb.AppendLine("Process(i) = [!" + temp + "]rule1.i.(i+1)%N{bullet[i]=1; leader[i]=1; shield[i]=1;} -> Process(i)");
            sb.AppendLine("\t\t\t [] [leader[i] == 0 && shield[i] == 1 && " + temp + "]\n\t\t\t\t\trule2.i.(i+1)%N{leader[i]=0; shield[i]=0; bullet[(i+1)%N] = 0; shield[(i+1)%N] = 1;} -> Process(i)");
            sb.AppendLine("\t\t\t [] [leader[i] == 1 && shield[i] == 1 && " + temp + "]\n\t\t\t\t\trule3.i.(i+1)%N{ bullet[i] = 1; leader[i] = 1; shield[i] = 0; bullet[(i+1)%N] = 0; shield[(i+1)%N] = 1;} -> Process(i)");
            sb.AppendLine("\t\t\t [] [leader[i] == 1 && shield[i] == 0 && bullet[(i + 1) % N] == 0 && " + temp + "]\n\t\t\t\t\trule4.i.(i+1)%N{ bullet[i] = 1; leader[i] = 1; shield[i]=0; bullet[(i+1) % N] = 0;} -> Process(i)");
            sb.AppendLine("\t\t\t [] [shield[i] == 0 && bullet[(i+1)% N] == 1 && " + temp + "]\n\t\t\t\t\trule5.i.(i+1)%N{bullet[i] = 1; leader[i] = 0; shield[i] = 0; bullet[(i+1)%N] = 0;} -> Process(i);");
            sb.AppendLine();

            sb.AppendLine("/* The Oracle*/");
            sb.AppendLine("DetectorCorrect() = [detectorcorrect == 0](progress{detectorcorrect = 1;} ->  DetectorCorrect());");
            sb.AppendLine("RandomDetector() = [detectorcorrect == 0]((guess1{detector = false;} -> RandomDetector()) [] (guess2{detector = true;} -> RandomDetector()));");
            sb.AppendLine();

            sb.AppendLine("Initialization() = ((tau{leader[0] = 0;} -> Skip) [] (tau{leader[0] = 1;} -> Skip));");
            for (int i = 1; i < N; i++)
            {
                sb.AppendLine("                   ((tau{leader[" + i + "] = 0;} -> Skip) [] (tau{leader[" + i + "] = 1;} -> Skip));");
            }
            for (int i = 0; i < N; i++)
            {
                sb.AppendLine("                   ((tau{bullet[" + i + "] = 0;} -> Skip) [] (tau{bullet[" + i + "] = 1;} -> Skip));");
            }
            for (int i = 0; i < N; i++)
            {
                sb.AppendLine("                   ((tau{shield[" + i + "] = 0;} -> Skip) [] (tau{shield[" + i + "] = 1;} -> Skip));");
            }

            sb.AppendLine();
            sb.AppendLine("LeaderElection() = Initialization(); (DetectorCorrect() ||| RandomDetector() |||");
            sb.Append("                   ");

            for (int i = 0; i < N; i++)
            {
                sb.Append("Process(" + i + ")|||");
            }

            sb.Remove(sb.Length - 3, 3);
            sb.Append(");");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("////////////////The Properties//////////////////");

            string leaders = "";
            for (int i = 0; i < N; i++)
            {
                if (i != N - 1)
                {
                    leaders += " leader[" + i + "] +";
                }
                else
                {
                    leaders += " leader[" + i + "]";
                }
            }

            sb.AppendLine("#define oneLeader (" + leaders + " == 1);");

            sb.AppendLine("#assert LeaderElection() |= <>[]oneLeader;");
            return sb.ToString();
        }

        public static string LoadLeaderElectionCompleteGraph(int N, bool isWL)
        {
            string leaders = "";
            for (int i = 0; i < N; i++)
            {
                if (i != N - 1)
                {
                    leaders += "leader[" + i + "] + ";
                }
                else
                {
                    leaders += "leader[" + i + "]";
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("////////////////The Model//////////////////");
            sb.AppendLine("#define N " + N + ";");
            sb.AppendLine();

            sb.AppendLine("var detectorcorrect = 0;");
            sb.AppendLine("var detector = false;");
            sb.AppendLine("var leader[N];");
            sb.AppendLine();

            sb.AppendLine("/*Rule 1*/");
            sb.AppendLine("Rule1(i, r) = [leader[i] == 1 && leader[r] == 1] ");
            if (isWL)
            {
                sb.AppendLine("			  (wf(rule1.i.r){leader[r] = 0;} -> Rule1(i, r)); ");
            }
            else
            {
                sb.AppendLine("			  (rule1.i.r{leader[r] = 0;} -> Rule1(i, r)); ");
            }
            sb.AppendLine();

            sb.AppendLine("/*Rule 2*/");
            sb.AppendLine("Rule2(i, r) = [leader[i] == 0 && leader[r] == 0 && !((detectorcorrect == 0 && detector) || (detectorcorrect != 0 && ( " + leaders + " > 0)))]");
            if (isWL)
            {
                sb.AppendLine("			  (wf(rule2.i.r){leader[i] = 1;} -> Rule2(i, r));");
            }
            else
            {
                sb.AppendLine("			   (rule2.i.r{leader[i] = 1;} -> Rule2(i, r));");
            }
            sb.AppendLine();

            sb.AppendLine("/*Rule 3*/");
            sb.AppendLine("Rule3(i, r) = [leader[i] == 0 && leader[r] == 0 && ((detectorcorrect == 0 && detector) || (detectorcorrect != 0 && (" + leaders + " > 0)))] ");
            if (isWL)
            {
                sb.AppendLine("			  (wf(rule3.i.r) -> Rule3(i, r));");
            }
            else
            {
                sb.AppendLine("			  (rule3.i.r -> Rule3(i, r));");
            }
            sb.AppendLine();

            sb.AppendLine("/* eventual leader detector*/");
            if (isWL)
            {
                sb.AppendLine("DetectorCorrect() = [detectorcorrect == 0](wf(progress){detectorcorrect = 1;} ->  DetectorCorrect());");
                sb.AppendLine("RandomDetector() = [detectorcorrect == 0]((wf(random1){detector = false;} -> RandomDetector()) [] (wf(random2){detector = true;} -> RandomDetector()));");
            }
            else
            {
                sb.AppendLine("DetectorCorrect() = [detectorcorrect == 0](progress{detectorcorrect = 1;} ->  DetectorCorrect());");
                sb.AppendLine("RandomDetector() = [detectorcorrect == 0]((random1{detector = false;} -> RandomDetector()) [] (random2{detector = true;} -> RandomDetector()));");
            }
            sb.AppendLine();

            sb.AppendLine("Initialization() = ((tau{leader[0] = 0;} -> Skip) [] (tau{leader[0] = 1;} -> Skip));");
            for (int i = 1; i < N; i++)
            {
                sb.AppendLine("                   ((tau{leader[" + i + "] = 0;} -> Skip) [] (tau{leader[" + i + "] = 1;} -> Skip));");
            }

            sb.AppendLine();
            sb.AppendLine("LeaderElection() = Initialization(); (DetectorCorrect() ||| RandomDetector() |||");
            sb.Append("                   ");

            for (int i = 0; i < N; i++)
            {
                for (int j = i + 1; j < N; j++)
                {
                    sb.Append("Rule1(" + i + "," + j + ")|||Rule1(" + j + "," + i + ")|||");
                }
            }
            sb.AppendLine();
            sb.Append("                   ");
            for (int i = 0; i < N; i++)
            {
                for (int j = i + 1; j < N; j++)
                {
                    sb.Append("Rule2(" + i + "," + j + ")|||Rule2(" + j + "," + i + ")|||");
                }
            }
            sb.AppendLine();
            sb.Append("                   ");
            for (int i = 0; i < N; i++)
            {
                for (int j = i + 1; j < N; j++)
                {
                    if (i == N - 2)
                    {
                        sb.Append("Rule3(" + i + "," + j + ")|||Rule3(" + j + "," + i + "));");
                    }
                    else
                    {
                        sb.Append("Rule3(" + i + "," + j + ")|||Rule3(" + j + "," + i + ")|||");
                    }
                }
            }
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("////////////////The Properties//////////////////");

            sb.AppendLine("#define oneLeader (" + leaders + " == 1);");

            sb.AppendLine("#assert LeaderElection() |= <>[]oneLeader;");
            return sb.ToString();
        }


        public static string LoadTokenCirculationinRings(int N)
        {
            string oneToken = "";
            for (int i = 0; i < N; i++)
            {
                if (i != N - 1)
                {
                    oneToken += "token[" + i + "] + ";
                }
                else
                {
                    oneToken += "token[" + i + "]";
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("////////////////The Model//////////////////");
            sb.AppendLine("#define N " + N + ";");
            sb.AppendLine();

            sb.AppendLine("////////////////Global Variables//////////////////");

            sb.AppendLine("var leader[N];");
            sb.AppendLine("var token[N];");
            sb.AppendLine("var label[N];");
            sb.AppendLine();

            sb.AppendLine("////////////////Rules//////////////////");

            sb.AppendLine("/*Rule 1*/");
            sb.AppendLine("Rule1(i, r) = [leader[i] == 0 && leader[r] == 1 && label[i] == label[r]]");
            sb.AppendLine("			     (rule1.i.r{token[i] = 0; token[r] = 1; label[r] = 1-label[i];} -> Rule1(i, r));");

            sb.AppendLine();

            sb.AppendLine("/*Rule 2*/");
            sb.AppendLine("Rule2(i, r) = [leader[r] == 0 && label[i] != label[r]]");
            sb.AppendLine("			  (rule2.i.r{token[i] = 0; token[r] = 1; label[r] = label[i];} -> Rule2(i, r));");
            sb.AppendLine();

            sb.Append("Initialization() = (tau{leader[0] = 1;");
            for (int i = 1; i < N; i++)
            {
                sb.Append("leader[" + i + "] = 0; ");
            }
            sb.AppendLine("}  -> Skip);");

            for (int i = 0; i < N; i++)
            {
                sb.AppendLine("                   ((tau{token[" + i + "] = 0;} -> Skip) [] (tau{token[" + i + "] = 1;} -> Skip));");
            }
            for (int i = 0; i < N; i++)
            {
                sb.AppendLine("                   ((tau{label[" + i + "] = 0;} -> Skip) [] (tau{label[" + i + "] = 1;} -> Skip));");
            }

            sb.AppendLine();
            sb.AppendLine("TokenCirculation() = Initialization();");
            sb.Append("                   (");

            for (int i = 0; i < N; i++)
            {
                sb.Append("Rule1(" + i + "," + (i + 1) % N + ")|||");
            }
            for (int i = 0; i < N - 1; i++)
            {
                sb.Append("Rule2(" + i + "," + (i + 1) + ")|||");
            }
            sb.AppendLine("Rule2(" + (N - 1) + ", 0));");



            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("////////////////The Properties//////////////////");

            sb.AppendLine("#define oneToken (" + oneToken + " == 1);");
            for (int i = 0; i < N; i++)
            {
                sb.AppendLine("#define token" + i + " (token[" + i + "] == 1);");
            }

            sb.Append("#assert TokenCirculation() |= ");
            for (int i = 0; i < N; i++)
            {
                sb.Append("[]<> (token" + i + " -> (");
                for (int j = 0; j < N - 2; j++)
                {
                    int index = (j + i + 2) % N;
                    if (j == N - 3)
                    {
                        sb.Append("!token" + index);
                    }
                    else
                    {
                        sb.Append("!token" + index + " && ");
                    }
                }
                if (i == N - 1)
                {
                    sb.Append(" U token" + (i + 1) % N + "));");
                }
                else
                {
                    sb.Append(" U token" + (i + 1) % N + ")) && ");
                }
            }

            sb.AppendLine();

            sb.AppendLine("#assert TokenCirculation() |= <>[]oneToken;");
            sb.Append("#assert TokenCirculation() |= ");
            for (int i = 0; i < N - 1; i++)
            {
                sb.Append("[]<> token" + i + " && ");
            }
            sb.Append("[]<> token" + (N - 1) + ";");

            sb.AppendLine();

            return sb.ToString();
        }

        public static string LoadLeaderElectionOddRing(int N)
        {
            string oneToken = "";
            for (int i = 0; i < N; i++)
            {
                if (i != N - 1)
                {
                    oneToken += "token[" + i + "] + ";
                }
                else
                {
                    oneToken += "token[" + i + "]";
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("////////////////The Model//////////////////");
            sb.AppendLine("#define N " + N + ";");
            sb.AppendLine();

            sb.AppendLine("////////////////Global Variables//////////////////");

            sb.AppendLine("var leader[N];");
            sb.AppendLine("var bullet[N];");
            sb.AppendLine("var label[N];");
            sb.AppendLine("var probe[N];");
            sb.AppendLine("var phase[N];");

            sb.AppendLine();

            sb.AppendLine("////////////////Processes//////////////////");
            sb.AppendLine("Process(i) = [label[i] == label[(i+1)%N] && probe[i] == 1 && phase[i] == 0 && leader[(i+1)%N] == 0 ]");
            sb.AppendLine("\t\t\t\t rule01.i.(i+1)%N{ leader[i] = 1; probe[i] = 0; phase[i] = 1; probe[(i+1)%N] = 1; } -> Process(i)");

            sb.AppendLine("\t\t\t [] [label[i] == label[(i+1)%N] && probe[i] == 1 && phase[i] == 0 && leader[(i+1)%N] == 1 ]");
            sb.AppendLine("\t\t\t\t rule02.i.(i+1)%N{ leader[i] = 1; probe[i] = 0; phase[i] = 1; } -> Process(i)");

            sb.AppendLine("\t\t\t [] [label[i] == label[(i+1)%N] && probe[i] == 1 && phase[i] == 1 && probe[(i+1)%N] == 0 ]");
            sb.AppendLine("\t\t\t\t rule03.i.(i+1)%N{ leader[i] = 1; probe[i] = 0; bullet[(i+1)%N] = 0; label[(i+1)%N] = 1-label[(i+1)%N]; phase[(i+1)%N] = 0; } -> Process(i)");

            sb.AppendLine("\t\t\t [] [label[i] == label[(i+1)%N] && probe[i] == 1 && phase[i] == 1 && probe[(i+1)%N] == 1 ]");
            sb.AppendLine("\t\t\t\t rule04.i.(i+1)%N{ leader[i] = 1; probe[i] = 0; } -> Process(i)");

            sb.AppendLine("\t\t\t [] [label[i] == label[(i+1)%N] && probe[i] == 0 && phase[i] == 0 && leader[(i+1)%N] == 0 ]");
            sb.AppendLine("\t\t\t\t rule05.i.(i+1)%N{ phase[i] = 1; probe[(i+1)%N] = 1; } -> Process(i)");

            sb.AppendLine("\t\t\t [] [label[i] == label[(i+1)%N] && probe[i] == 0 && phase[i] == 0 && leader[(i+1)%N] == 1 ]");
            sb.AppendLine("\t\t\t\t rule06.i.(i+1)%N{ phase[i] = 1; } -> Process(i)");

            sb.AppendLine("\t\t\t [] [label[i] == label[(i+1)%N] && probe[i] == 0 && phase[i] == 1 && probe[(i+1)%N] == 0 ]");
            sb.AppendLine("\t\t\t\t rule07.i.(i+1)%N{ bullet[(i+1)%N] = 0; label[(i+1)%N] = 1-label[(i+1)%N]; phase[(i+1)%N] = 0; } -> Process(i)");

            sb.AppendLine("\t\t\t [] [label[i] != label[(i+1)%N] && leader[(i+1)%N] == 1 && bullet[(i+1)%N] == 1  ]");
            sb.AppendLine("\t\t\t\t rule08.i.(i+1)%N{ leader[(i+1)%N] = 0; } -> Process(i)");

            sb.AppendLine("\t\t\t [] [label[i] != label[(i+1)%N] && leader[(i+1)%N] == 1 && bullet[(i+1)%N] == 0  ]");
            sb.AppendLine("\t\t\t\t rule09.i.(i+1)%N{ bullet[i] = 1; probe[i] = 0;} -> Process(i)");

            sb.AppendLine("\t\t\t [] [label[i] != label[(i+1)%N] && leader[(i+1)%N] == 0 && bullet[(i+1)%N] == 1 && probe[i] == 1]");
            sb.AppendLine("\t\t\t\t rule10.i.(i+1)%N{ bullet[i] = 1; bullet[(i+1)%N] = 0;  probe[i] = 0; probe[(i+1)%N] = 1;} -> Process(i)");

            sb.AppendLine("\t\t\t [] [label[i] != label[(i+1)%N] && leader[(i+1)%N] == 0 && bullet[(i+1)%N] == 0 && probe[i] == 1]");
            sb.AppendLine("\t\t\t\t rule11.i.(i+1)%N{ probe[i] = 0; probe[(i+1)%N] = 1; } -> Process(i)");

            sb.AppendLine("\t\t\t [] [label[i] != label[(i+1)%N] && leader[(i+1)%N] == 0 && bullet[(i+1)%N] == 1 && probe[i] == 0]");
            sb.AppendLine("\t\t\t\t rule12.i.(i+1)%N{ bullet[i] = 1; bullet[(i+1)%N] = 0; } -> Process(i);");

            sb.AppendLine();


            sb.AppendLine("Initialization() = ((tau{leader[0] = 0;} -> Skip) [] (tau{leader[0] = 1;} -> Skip));");
            for (int i = 1; i < N; i++)
            {
                sb.AppendLine("                   ((tau{leader[" + i + "] = 0;} -> Skip) [] (tau{leader[" + i + "] = 1;} -> Skip));");
            }
            for (int i = 0; i < N; i++)
            {
                sb.AppendLine("                   ((tau{bullet[" + i + "] = 0;} -> Skip) [] (tau{bullet[" + i + "] = 1;} -> Skip));");
            }
            for (int i = 0; i < N; i++)
            {
                sb.AppendLine("                   ((tau{label[" + i + "] = 0;} -> Skip) [] (tau{label[" + i + "] = 1;} -> Skip));");
            }
            for (int i = 0; i < N; i++)
            {
                sb.AppendLine("                   ((tau{probe[" + i + "] = 0;} -> Skip) [] (tau{probe[" + i + "] = 1;} -> Skip));");
            }
            for (int i = 0; i < N; i++)
            {
                sb.AppendLine("                   ((tau{phase[" + i + "] = 0;} -> Skip) [] (tau{probe[" + i + "] = 1;} -> Skip));");
            }

            sb.AppendLine();
            sb.Append("LeaderElectionOddRing() = Initialization(); (");


            for (int i = 0; i < N; i++)
            {
                sb.Append("Process(" + i + ")|||");
            }

            sb.Remove(sb.Length - 3, 3);
            sb.Append(");");

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("////////////////The Properties//////////////////");

            string leaders = "";
            for (int i = 0; i < N; i++)
            {
                if (i != N - 1)
                {
                    leaders += " leader[" + i + "] +";
                }
                else
                {
                    leaders += " leader[" + i + "]";
                }
            }

            sb.AppendLine("#define oneLeader (" + leaders + " == 1);");

            sb.AppendLine("#assert LeaderElectionOddRing() |= <>[]oneLeader;");
            return sb.ToString();
        }


        public static string LoadLeaderElectionDirectTree(int N)
        {
            string leaders = "";
            for (int i = 0; i < N; i++)
            {
                if (i != N - 1)
                {
                    leaders += "leader[" + i + "] + ";
                }
                else
                {
                    leaders += "leader[" + i + "]";
                }
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* Self-stabilizing leader election in (directed) rooted trees */");
            sb.AppendLine("////////////////The Model//////////////////");
            sb.AppendLine("#define N " + N + ";");
            sb.AppendLine();

            sb.AppendLine("var detectorcorrect = 0;");
            sb.AppendLine("var detector = false;");
            sb.AppendLine("var leader[N];");
            sb.AppendLine();

            sb.AppendLine("/*Rule 1*/");
            sb.AppendLine("Rule1(i, r) = [leader[i] == 1 && leader[r] == 1]");
            sb.AppendLine("			     (rule1.i.r{leader[r] = 0;} -> Rule1(i, r));");
            sb.AppendLine();

            sb.AppendLine("/*Rule 2*/");
            sb.AppendLine("Rule2(i, r) = [leader[i] == 0 && leader[r] == 0 && !((detectorcorrect == 0 && detector) || (detectorcorrect != 0 && (  leader[0] + leader[1] + leader[2] > 0)))]");
            sb.AppendLine("			     (rule2.i.r{leader[i] = 1;} -> Rule2(i, r));");
            sb.AppendLine();

            sb.AppendLine("/*Rule 3*/");
            sb.AppendLine("Rule3(i, r) = [leader[i] == 0 && leader[r] == 1]");
            sb.AppendLine("			     (rule3.i.r{leader[i] = 1; leader[r] = 0;} -> Rule3(i, r));");
            sb.AppendLine();

            sb.AppendLine("/* eventual leader detector*/");
            sb.AppendLine("DetectorCorrect() = [detectorcorrect == 0](progress{detectorcorrect = 1;} ->  DetectorCorrect());");
            sb.AppendLine("RandomDetector() = [detectorcorrect == 0]((random1{detector = false;} -> RandomDetector()) [] (random2{detector = true;} -> RandomDetector()));");
            sb.AppendLine();

            sb.AppendLine("Initialization() = ((tau{leader[0] = 0;} -> Skip) [] (tau{leader[0] = 1;} -> Skip));");
            for (int i = 1; i < N; i++)
            {
                sb.AppendLine("                   ((tau{leader[" + i + "] = 0;} -> Skip) [] (tau{leader[" + i + "] = 1;} -> Skip));");
            }

            sb.AppendLine();
            sb.AppendLine("/* The topology is a rooted tree. */");
            sb.AppendLine("LeaderElection() = Initialization(); (DetectorCorrect() ||| RandomDetector() |||");
            sb.Append("");

            int j = 0;
            for (int i = 0; i + j + 1 < N; i++)
            {
                sb.AppendLine("                   Rule1(" + i + "," + (i + j + 1) + ")|||Rule1(" + i + "," + (i + j + 2) + ")|||");
                sb.AppendLine("                   Rule2(" + i + "," + (i + j + 1) + ")|||Rule2(" + i + "," + (i + j + 2) + ")|||");
                sb.AppendLine("                   Rule3(" + i + "," + (i + j + 1) + ")|||Rule3(" + i + "," + (i + j + 2) + ")|||");
                j = j + 1;
            }
            sb.Remove(sb.Length - 5, 5);
            sb.AppendLine(");");
            sb.AppendLine();
            sb.AppendLine("////////////////The Properties//////////////////");

            sb.AppendLine("#define oneLeader (" + leaders + " == 1);");

            sb.AppendLine("#assert LeaderElection() |= <>[]oneLeader;");
            return sb.ToString();
        }


        public static string LoadRegisterSimulation(int N, bool isCorrect, bool showDetailedAction)
        {
            string write_i_0, write_i_1, read, write_i;
            if (showDetailedAction)
            {
                write_i_0 = "write.i.0";
                write_i_1 = "write.i.1";
                read = "read";
                write_i = "write.i";
            }
            else
            {
                write_i_0 = "tau";
                write_i_1 = "tau";
                read = "tau";
                write_i = "tau";
            }


            StringBuilder sb = new StringBuilder();
            sb.AppendLine("//shared binary array of size N");
            sb.Append("var B = [");
            for (int i = 0; i < N - 1; i++)
            {
                sb.Append("0,");
            }
            sb.AppendLine("1];");
            sb.AppendLine("//register value used by the abstract model");
            sb.AppendLine("var R = " + (N - 1) + ";");
            sb.AppendLine("//temporary value for abstract reader to stored the value read from register");
            sb.AppendLine("var M = 0;");
            sb.AppendLine();

            sb.AppendLine("////////////////The Concrete Implementation Model//////////////////");
            sb.AppendLine("Readers() = read_inv -> UpScan(0);");
            if (isCorrect)
            {
                sb.AppendLine("UpScan(i) = if(B[i] == 1) { DownScan(i - 1, i) } else { UpScan(i + 1) };");

                sb.AppendLine("DownScan(i, v) =");
                sb.AppendLine("\t\tif(i >= 0) {");
                sb.AppendLine("\t\t\tif(B[i] == 1) { DownScan(i - 1, i) } else { DownScan(i - 1, v) }");
                sb.AppendLine("\t\t} else {");
                sb.AppendLine("\t\t\tread_res.v -> Readers()");
                sb.AppendLine("\t\t};");
            }
            else
            {
                sb.AppendLine("UpScan(i) = if(B[i] == 1) { read_res.i -> Readers() } else { UpScan(i + 1) };");
            }
            sb.AppendLine();

            sb.AppendLine("Writer(i) = write_inv.i -> " + write_i_1 + "{B[i] = 1;} -> WriterDownScan(i-1);");
            sb.AppendLine("WriterDownScan(i) = if(i >= 0) { " + write_i_0 + "{B[i] = 0;} -> WriterDownScan(i-1) } else { write_res -> Skip };");
            sb.AppendLine();

            sb.Append("Writers() = (");
            for (int i = 0; i < N - 1; i++)
            {
                sb.Append("Writer(" + i + ")[]");
            }
            sb.AppendLine("Writer(" + (N - 1) + ")); Writers();");

           

            if (showDetailedAction)
            {
                sb.Append("Register() = (Readers() ||| Writers()) \\{");
                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        sb.Append("write." + i + "." + j + ", ");
                    }
                }
                sb.Remove(sb.Length - 2, 2);
                sb.AppendLine("};");
            }
            else
            {
                sb.Append("Register() = Readers() ||| Writers();");
            }

            sb.AppendLine();

            sb.AppendLine("////////////////The Abstract Specification Model//////////////////");
            sb.AppendLine("ReadersAbs() = read_inv -> " + read + "{M=R;} -> read_res.M -> ReadersAbs();");          
            sb.AppendLine();

            sb.AppendLine("WriterAbs(i) = write_inv.i -> " + write_i + "{R=i;} -> write_res -> Skip;");
            sb.Append("WritersAbs() = (");

            for (int i = 0; i < N - 1; i++)
            {
                sb.Append("WriterAbs(" + i + ")[]");
            }
            sb.AppendLine("WriterAbs(" + (N - 1) + ")); WritersAbs();");

            sb.AppendLine();
            
            if (showDetailedAction)
            {
                sb.Append("RegisterAbs() = (ReadersAbs() ||| WritersAbs()) \\{");
                for (int i = 0; i < N; i++)
                {
                    sb.Append("write." + i + ", ");
                }

                sb.AppendLine("read};");
            }
            else
            {
                sb.Append("RegisterAbs() = ReadersAbs() ||| WritersAbs();");
            }

            sb.AppendLine();

            sb.AppendLine("////////////////The Properties//////////////////");
            sb.AppendLine("#assert Register() refines RegisterAbs();");
            sb.AppendLine("#assert RegisterAbs() refines Register();");
            return sb.ToString();
        }


        public static string LoadRegisterSimulationMoreReader(int N, int ReaderNumber, bool isCorrect)
        {
            string write_i_0 = "tau";
            string write_i_1 = "tau";
            string read = "tau";
            string write_i = "tau";
           

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("#define N " + ReaderNumber + ";");
            sb.AppendLine();

            sb.AppendLine("//shared binary array of size N");
            sb.Append("var B = [");
            for (int i = 0; i < N - 1; i++)
            {
                sb.Append("0,");
            }
            sb.AppendLine("1];");
            sb.AppendLine("//register value used by the abstract model");
            sb.AppendLine("var R = " + (N - 1) + ";");
            sb.AppendLine("//temporary value for abstract reader to stored the value read from register");
            sb.AppendLine("var M[N];");
            sb.AppendLine();

            sb.AppendLine("////////////////The Concrete Implementation Model//////////////////");
            sb.AppendLine("Readers(id) = read_inv.id -> UpScan(0, id);");
            if (isCorrect)
            {
                sb.AppendLine("UpScan(i, id) =  if(B[i] == 1) { DownScan(i - 1, i, id) } else { UpScan(i + 1, id) };");
                sb.AppendLine("DownScan(i, v, id) =");
                sb.AppendLine("\t\tif(i >= 0) {");
                sb.AppendLine("\t\t\tif(B[i] == 1) { DownScan(i - 1, i, id) } else { DownScan(i - 1, v, id) }");
                sb.AppendLine("\t\t} else {");
                sb.AppendLine("\t\t\tread_res.id.v -> Readers(id)");
                sb.AppendLine("\t\t};");
            }
            else
            {
                sb.AppendLine("UpScan(i, id) =  if(B[i] == 1) { read_res.id.i -> Readers(id) } else { UpScan(i + 1, id) };");
            }
            sb.AppendLine();

            sb.AppendLine("Writer(i) = write_inv.i -> " + write_i_1 + "{B[i] = 1;} -> WriterDownScan(i-1);");
            sb.AppendLine("WriterDownScan(i) = if(i >= 0 ) { " + write_i_0 + "{B[i] = 0;} -> WriterDownScan(i-1) } else { write_res -> Skip } ;");
            sb.AppendLine();

            sb.Append("Writers() = (");
            for (int i = 0; i < N - 1; i++)
            {
                sb.Append("Writer(" + i + ")[]");
            }
            sb.AppendLine("Writer(" + (N - 1) + ")); Writers();");

            sb.Append("Register() = (|||x:{0..N-1}@Readers(x)) ||| Writers();");

            sb.AppendLine();

            sb.AppendLine("////////////////The Abstract Specification Model//////////////////");
            sb.AppendLine("ReadersAbs(id) = read_inv.id -> " + read + "{M[id]=R;} -> read_res.id.M[id] -> ReadersAbs(id);");
            sb.AppendLine();

            sb.AppendLine("WriterAbs(i) = write_inv.i -> " + write_i + "{R=i;} -> write_res -> Skip;");
            sb.Append("WritersAbs() = (");

            for (int i = 0; i < N - 1; i++)
            {
                sb.Append("WriterAbs(" + i + ")[]");
            }
            sb.AppendLine("WriterAbs(" + (N - 1) + ")); WritersAbs();");

            sb.AppendLine();
            sb.Append("RegisterAbs() = (|||x:{0..N-1}@ReadersAbs(x))||| WritersAbs();");
            
            sb.AppendLine();

            sb.AppendLine("////////////////The Properties//////////////////");
            sb.AppendLine("#assert Register() refines RegisterAbs();");
            sb.AppendLine("#assert RegisterAbs() refines Register();");
            return sb.ToString();
        }

        public static string LoadStackImplementationLinearPoint(int N, int stackSize)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("////number of processes");
            sb.AppendLine("#define N " + N + ";");
            sb.AppendLine("//stack size");
            sb.AppendLine("#define SIZE " + stackSize + ";");
            sb.AppendLine();
            sb.AppendLine("//shared head pointer for the concrete implementation");
            sb.AppendLine("var H = 0;");
            sb.AppendLine("//local variable to store the temporary head value");
            sb.AppendLine("var HL[N];");
            sb.AppendLine("//shared head pointer for the abstract implementation");
            sb.AppendLine("var HA = 0;");
            sb.AppendLine("");
            sb.AppendLine("////////////////The Concrete Implementation Model//////////////////");
            sb.AppendLine("Push(i) = ");
            sb.AppendLine("	    tau{HL[i]=H;} -> ");
            sb.AppendLine("	    ifa (HL[i] == H) {");
            sb.AppendLine("		    push.i.(H+1){if(H < SIZE) {H = H+1;}} -> Skip");
            sb.AppendLine("	    } else {");
            sb.AppendLine("	    	tau -> Push(i)");
            sb.AppendLine("  	};");
            sb.AppendLine("");
            sb.AppendLine("Pop(i) =");  
            sb.AppendLine("		tau{HL[i]=H;} -> ");
            sb.AppendLine("		ifa(H == 0) {");
            sb.AppendLine("			pop.i.0 -> Skip ");
            sb.AppendLine("		} else {");
            sb.AppendLine("			tau-> ifa(HL[i] != H) {tau -> Pop(i) } else {pop.i.H{if(H > 0) {H = H -1;}} -> Skip}");
            sb.AppendLine("		};");
            sb.AppendLine("");
            sb.AppendLine("Process(i) = (Push(i)[]Pop(i));Process(i);");
            sb.AppendLine("Stack() = (|||x:{0..N-1}@Process(x));");
            sb.AppendLine("");
            sb.AppendLine("////////////////The Abstract Specification Model//////////////////");
            sb.AppendLine("PushAbs(i) = push.i.(HA + 1) {if(HA < SIZE) {HA = HA+1;}} -> Skip;");
            sb.AppendLine("PopAbs(i) = pop.i.HA{if(HA > 0) {HA = HA -1;}} -> Skip;");
            sb.AppendLine("");
            sb.AppendLine("ProcessAbs(i) = (PushAbs(i)[]PopAbs(i));ProcessAbs(i);");
            sb.AppendLine("");
            sb.AppendLine("StackAbs() = (|||x:{0..N-1}@ProcessAbs(x));");
            sb.AppendLine("");
            sb.AppendLine("////////////////The Properties//////////////////");
            sb.AppendLine("#assert Stack() refines StackAbs();");
            sb.AppendLine("#assert StackAbs() refines Stack();");
            return sb.ToString();
        }

        public static string LoadTest317()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("P = a -> b -> P []a -> c -> P [] a -> d -> P [] a -> e -> P [] a -> f -> P;");
            sb.AppendLine("Q = a -> Q;");
            sb.AppendLine("Sys = (P||Q) \\{a};");
            sb.AppendLine("");
            sb.AppendLine("#assert Sys deadlockfree;");

            return sb.ToString();
        }

        public static string LoadStackImplementation(int N, int stackSize, bool showDetailedAction)
        {
            string headread_i, push_i, pop_i, pop_empty_i;
            if (showDetailedAction)
            {
                headread_i = "headread.i";
                push_i = "push.i";
                pop_i = "pop.i";
                pop_empty_i = "pop_empty.i";

            }
            else
            {
                headread_i = "tau";
                push_i = "tau";
                pop_i = "tau";
                pop_empty_i = "tau"; 
            }


            StringBuilder sb = new StringBuilder();
            sb.AppendLine("////number of processes");
            sb.AppendLine("#define N " + N + ";");
            sb.AppendLine("//stack size");
            sb.AppendLine("#define SIZE " + stackSize + ";");
            sb.AppendLine();

            sb.AppendLine("//shared head pointer for the concrete implementation");
            sb.AppendLine("var H = 0;");
            sb.AppendLine("//local variable to store the temporary head value");
            sb.AppendLine("var HL[N];");
            sb.AppendLine();
            sb.AppendLine("//shared head pointer for the abstract implementation");
            sb.AppendLine("var HA = 0;");
            sb.AppendLine("//local variable to store the temporary head value");
            sb.AppendLine("var HLA[N];");
            sb.AppendLine();

            string s = "";
            sb.AppendLine("////////////////The Concrete Implementation Model//////////////////");
            sb.AppendLine("PushLoop(i) = " + headread_i + "{HL[i]=H;} -> (");
            sb.AppendLine("\tifa (HL[i] == H) {");
            sb.AppendLine("\t\t" + push_i + "{if(H < SIZE) {H = H+1;} HL[i]=H;} -> tau -> push_res.i.HL[i] -> Skip");
            sb.AppendLine("\t} else {");
            sb.AppendLine("\t\tPushLoop(i)");
            sb.AppendLine("\t});");
            sb.AppendLine();

            sb.AppendLine("PopLoop(i) = " + headread_i + "{HL[i]=H;} -> ");
            sb.AppendLine("\t(if(HL[i] == 0) {");
            sb.AppendLine("\t\tpop_res.i.0 -> Skip ");
            sb.AppendLine("\t} else {");
            sb.AppendLine("\t\t(ifa(HL[i] != H) { PopLoop(i) } else { " + pop_i + "{H = H-1; HL[i]=H;} -> tau -> pop_res.i.(HL[i]+1) -> Skip");
            sb.AppendLine("\t\t})");
            sb.AppendLine("\t});");
            sb.AppendLine();


            sb.AppendLine("Process(i) = (push_inv.i -> PushLoop(i)[] pop_inv.i -> PopLoop(i));Process(i);");
            if (showDetailedAction)
            {
                sb.Append("Stack() = (|||x:{0..N-1}@Process(x)) \\ {");
                for (int i = 0; i < N; i++)
                {
                    sb.Append("headread." + i + ", push." + i + ", pop." + i + ", ");
                }
                sb.Remove(sb.Length - 2, 2);
                sb.AppendLine("};");
            }
            else
            {
                sb.Append("Stack() = |||x:{0..N-1}@Process(x);");
            }

            sb.AppendLine();

            sb.AppendLine("////////////////The Abstract Specification Model//////////////////");
            sb.AppendLine("PushAbs(i) = push_inv.i -> " + push_i + "{if(HA < SIZE) {HA = HA+1;}; HLA[i]=HA;} -> push_res.i.HLA[i] -> Skip;");
            sb.AppendLine();

            sb.AppendLine("PopAbs(i) = pop_inv.i ->");
            sb.AppendLine("\t(ifa(HA == 0) {");
            sb.AppendLine("\t\ttau-> pop_res.i.0 -> Skip ");
            sb.AppendLine("\t} else {");
            sb.AppendLine("\t\t " + pop_empty_i + "{HA = HA -1; HLA[i]=HA;} -> pop_res.i.(HLA[i]+1) -> Skip");
            sb.AppendLine("\t});");

            sb.AppendLine();
            sb.AppendLine("ProcessAbs(i) = (PushAbs(i)[]PopAbs(i));ProcessAbs(i);");

            sb.AppendLine();

            if (showDetailedAction)
            {
                sb.Append("StackAbs() = (|||x:{0..N-1}@ProcessAbs(x)) \\{");
                for (int i = 0; i < N; i++)
                {
                    sb.Append("push." + i + ", pop." + i + ", pop_empty." + i + ", ");
                }
                sb.Remove(sb.Length - 2, 2);
                sb.AppendLine("};");
            }
            else
            {
                sb.Append("StackAbs() = |||x:{0..N-1}@ProcessAbs(x);");
            }
            

            sb.AppendLine();

            sb.AppendLine("////////////////The Properties//////////////////");
            sb.AppendLine("#assert Stack() refines StackAbs();");
            sb.AppendLine("#assert StackAbs() refines Stack();");
            return sb.ToString();
        }

        public static string LoadSNIZ(int N, int nodes, int operationNumber, bool detail)
        {
            StringBuilder sb = new StringBuilder();

            if (!detail)
            {
                sb.AppendLine("//This model is for SNZI:Scalable NonZero Indicators");
                sb.AppendLine("");
                sb.AppendLine("//number of processes");
                sb.AppendLine("#define P 2;");
                sb.AppendLine("");
                sb.AppendLine("// number of nodes");
                sb.AppendLine("#define N 2;");
                sb.AppendLine("");
                sb.AppendLine("");
                sb.AppendLine("//------------------------------shared variable------------------------------");
                sb.AppendLine("");
                sb.AppendLine(
                    "//Since SNZI algorithm is organized as a rooted tree of SNZI objects, we create a N array of Node objects.");
                sb.AppendLine(
                    "//The root is the first element of the array,  and for 0 < i < N, the parent of the ith node is the (i-1)/2th node.");
                sb.AppendLine("");
                sb.AppendLine("//array of shared variables of nodes (including hierarchical and root nodes), ");
                sb.AppendLine("//each nodes has its own local copy. so there are N copies ");
                sb.AppendLine("var c[N];  //the original X.c");
                sb.AppendLine("var v[N];  //the original X.v");
                sb.AppendLine("");
                sb.AppendLine("");
                sb.AppendLine("var a = false;    //the origina X.a for the root, i.e. the first element of the array");
                sb.AppendLine("");
                sb.AppendLine("//presence indicator");
                sb.AppendLine("var I = false;");
                sb.AppendLine("");
                sb.AppendLine("");
                sb.AppendLine("//------------------------------local variable------------------------------");
                sb.AppendLine("");
                sb.AppendLine(
                    "//array of local variables which are used in the corresponding operations of nodes when a process arrives at or departs from nodes, ");
                sb.AppendLine("// i.e. representing x in the original algorithm");
                sb.AppendLine(
                    "//as there may be N processes visiting one node concurrently, there could be N * P local variables");
                sb.AppendLine("//this is a variant of 2-dimention array. for node i, the local variable of process j");
                sb.AppendLine("//can be calculated by (i * P + j)");
                sb.AppendLine("var cc[N * P];");
                sb.AppendLine("var vv[N * P];");
                sb.AppendLine("");
                sb.AppendLine(
                    "//aa is the local variable of root node, so there will at most P copies because only at most");
                sb.AppendLine("//P processes can visit root node at the same time.");
                sb.AppendLine("var aa[P];");
                sb.AppendLine("");
                sb.AppendLine("");
                sb.AppendLine("");
                sb.AppendLine("//another local variables of root node, representing x' in the original algorithm");
                sb.AppendLine("//As above, only at most P processes can visit the root node concurrently,");
                sb.AppendLine("//so each array contains P elements.");
                sb.AppendLine("var rc[P];");
                sb.AppendLine("var ra[P];");
                sb.AppendLine("var rv[P];");
                sb.AppendLine("");
                sb.AppendLine("");
                sb.AppendLine("//other local variables of hierarchical SNZI node");
                sb.AppendLine("//for each such node, P processes can visit it simultoneously, ");
                sb.AppendLine("//so the total number of each local varaible should be N * P");
                sb.AppendLine("var s[N * P];    //the original succ in the Arrive operation ");
                sb.AppendLine("var u[N * P]; //the original undoArr in the Arrive operation");
                sb.AppendLine("");
                sb.AppendLine("//for LL-SC primitive");
                sb.AppendLine("var count;");
                sb.AppendLine("var counts[P];");
                sb.AppendLine("");
                sb.AppendLine("");
                sb.AppendLine(
                    "//------------------------------The Concrete Implementation Model------------------------------");
                sb.AppendLine("");
                sb.AppendLine("//Single Entry of Arrival and Departure operations on any nodes ");
                sb.AppendLine("AI(p, n) = ai.p-> AG(p,n); ar.p -> Skip;");
                sb.AppendLine("");
                sb.AppendLine("DI(p, n) = di.p -> DG(p,n); dr.p -> Skip;");
                sb.AppendLine("");
                sb.AppendLine("AG(p, n) = ifa (n == 0) {ArriveR(p)} else { Arrive(p, n)};");
                sb.AppendLine("");
                sb.AppendLine("DG(p,n) = ifa (n == 0) {DepartR(p)} else {Depart(p,n)};");
                sb.AppendLine("");
                sb.AppendLine(
                    "//------------------------------start - this part is for root node------------------------------");
                sb.AppendLine("//Arrival on root node");
                sb.AppendLine("ArriveR(p) = 	");
                sb.AppendLine("	tau {cc[p] = c[0]; aa[p] = a; vv[p] = v[0];} ->// x <- Read(X)");
                sb.AppendLine("    if (cc[p] == 0) { // if x.c = 0    ");
                sb.AppendLine("		tau {rc[p] = 1; ra[p] = true; rv[p] = vv[p] + 1;} -> ");
                sb.AppendLine("			Until(p); Skip //  x'<- (1, true, x.v + 1)");
                sb.AppendLine("	} else {");
                sb.AppendLine("		 tau {rc[p] = cc[p] + 1; ra[p] = aa[p]; rv[p] = vv[p];} ->");
                sb.AppendLine("			Until(p); Skip // x'<-(x.c+1, x.a, x.v)							");
                sb.AppendLine("	};");
                sb.AppendLine("");
                sb.AppendLine("Until(p) =  ");
                sb.AppendLine("	ifa (cc[p] == c[0]&& aa[p] == a&& vv[p] == v[0])  //  until CAS(X, x, x')");
                sb.AppendLine("	 { tau {c[0] = rc[p]; a= ra[p]; v[0] = rv[p];} -> ");
                sb.AppendLine("	   if (ra[p] == true) // if x'.a then");
                sb.AppendLine("		{ tau {I = true; count = count + 1;} -> ");
                sb.AppendLine("		  ifa (c[0] == rc[p] && a== ra[p] && v[0] == rv[p])  //CAS(X,x',(x'.c, false, x'.v))");
                sb.AppendLine("			{ tau {c[0] = rc[p]; a= false; v[0] = rv[p];} -> Skip");
                sb.AppendLine("			} //else {tau  -> Skip}");
                sb.AppendLine("		} ");
                sb.AppendLine("	 }else {tau  -> ArriveR(p)};");
                sb.AppendLine("");
                sb.AppendLine("");
                sb.AppendLine("//Departure from root node");
                sb.AppendLine("DepartR(p) =  ");
                sb.AppendLine("		tau {cc[p] = c[0]; aa[p] = a; vv[p] = v[0];} ->");
                sb.AppendLine(
                    "		ifa (cc[p] == c[0] && aa[p] == a&& vv[p] == v[0]) //if CAS(X, x, (x.c - 1, false, x.v))");
                sb.AppendLine("		{ tau {c[0] = cc[p] - 1; a= false; v[0] = vv[p];} -> ");
                sb.AppendLine("		  if (cc[p] > 1) { Skip}  ");
                sb.AppendLine(" 		  else { DepartRL(p) }");
                sb.AppendLine("		}else { tau  -> DepartR(p)};   ");
                sb.AppendLine("");
                sb.AppendLine("											");
                sb.AppendLine("DepartRL(p) = tau { counts[p] = count;} -> // LL(I)");
                sb.AppendLine("			 if (vv[p] != v[0]) {  Skip}");
                sb.AppendLine("			 else");
                sb.AppendLine("			 {ifa ( counts[p] != count) {tau  -> DepartRL(p)}");
                sb.AppendLine("			  else {tau {I = false; count = count + 1;} -> Skip}");
                sb.AppendLine("			 };");
                sb.AppendLine(
                    "//------------------------------end - this part is for root node------------------------------");
                sb.AppendLine("");
                sb.AppendLine("		");
                sb.AppendLine("		");
                sb.AppendLine("		");
                sb.AppendLine(
                    "//------------------------------start - hierarchical SNZI node------------------------------");
                sb.AppendLine("//Arrival of hierarchical SNZI node");
                sb.AppendLine(
                    "Arrive(p, n) = tau {s[n * P + p] = false;} -> tau {u[n * P + p] = 0;} -> ArriveL1(p, n); ArriveL2(p, n); //Skip;");
                sb.AppendLine("");
                sb.AppendLine("ArriveL1(p, n) = ");
                sb.AppendLine("			if (!s[n * P + p]) ");
                sb.AppendLine("			{  tau {cc[n * P + p] = c[n]; vv[n * P + p] = v[n];} -> ");
                sb.AppendLine("			   if (cc[n * P + p] > 1)  //if x.c >= 1 then");
                sb.AppendLine("			   { ");
                sb.AppendLine(
                    "				 ifa (cc[n * P + p] == c[n] && vv[n * P + p] == v[n]) //if CAS(X, x, (x.c + 1, x.v)) then ");
                sb.AppendLine("				 { tau {c[n] = cc[n * P + p] + 2; v[n] = vv[n * P + p];} -> ");
                sb.AppendLine("				   tau {s[n * P + p] = true;} -> Case2(p, n)");
                sb.AppendLine("				 }else { tau  -> Case2(p, n)}");
                sb.AppendLine("			   }else { Case2(p, n)}");
                sb.AppendLine("			} ;//else {Skip};");
                sb.AppendLine("		");
                sb.AppendLine("Case2(p, n) = if (cc[n * P + p]== 0) // if x.c = 0 then");
                sb.AppendLine("			  { ");
                sb.AppendLine(
                    "				ifa (cc[n * P + p] == c[n] && vv[n * P + p] == v[n])  // if CAS(X, x, (1 / 2, x.v + 1)) then");
                sb.AppendLine("				{ tau {c[n] = 1; v[n] = vv[n * P + p] + 1;} -> tau {s[n * P + p] = true;} ->");
                sb.AppendLine("				  tau {cc[n * P + p] = 1; vv[n * P + p] = vv[n * P + p] + 1;} -> Case3(p, n)");
                sb.AppendLine("				}else { tau  -> Case3(p, n)}");
                sb.AppendLine("		  	 }else { Case3(p, n)};");
                sb.AppendLine("");
                sb.AppendLine("//if x.c = 1/2 then");
                sb.AppendLine("Case3(p, n) = if (cc[n * P + p] == 1) ");
                sb.AppendLine("			  {  AG(p, (n - 1)/2);");
                sb.AppendLine("				ifa (cc[n * P + p] == c[n] && vv[n * P + p] == v[n]) ");
                sb.AppendLine("				{ tau{c[n] = 2; v[n] = vv[n * P + p];} -> ArriveL1(p, n)");
                sb.AppendLine("				}else {tau  -> tau{u[n * P + p] = u[n * P + p] + 1;} -> ArriveL1(p, n)}");
                sb.AppendLine("			  }else { ArriveL1(p, n)};");
                sb.AppendLine("");
                sb.AppendLine("	 		 ");
                sb.AppendLine(
                    "ArriveL2(p, n) = if (u[n * P + p] > 0) { DG(p, (n - 1)/2);tau{u[n * P + p] = u[n * P + p] - 1;} ->ArriveL2(p, n)}");
                sb.AppendLine("				 else { Skip};");
                sb.AppendLine("");
                sb.AppendLine("//Departure of hierarchical SNZI node");
                sb.AppendLine("Depart(p, n) =  ");
                sb.AppendLine("			tau {cc[n * P + p] = c[n]; vv[n * P + p] = v[n];} -> ");
                sb.AppendLine(
                    "			ifa (cc[n * P + p] == c[n] && vv[n * P + p] == v[n]) //if CAS(X, x, (x.c - 1, x.v)) then");
                sb.AppendLine("			{ tau{c[n] = cc[n * P + p] - 2; v[n] = vv[n * P + p];} -> ");
                sb.AppendLine("			  if (cc[n * P + p] == 2) ");
                sb.AppendLine("			  { DG(p, (n - 1)/2)}");
                sb.AppendLine("			  //else { Skip}");
                sb.AppendLine("			}else {tau  -> Depart(p, n)};");
                sb.AppendLine("");
                sb.AppendLine("");
                sb.AppendLine(
                    "//------------------------------end - hierarchical SNZI node------------------------------");
                sb.AppendLine("		");
                sb.AppendLine("Pro(i, j) = [j <2]([] x:{0..N - 1}@(AI(i, x);DI(i, x));Pro(i, j+1)); ");
                sb.AppendLine("");
                sb.AppendLine("Q() = q.I -> Q();");
                sb.AppendLine("");
                sb.AppendLine("SNZI() = (|||x:{0..P - 1}@Pro(x, 0)) ||| Q();");
                sb.AppendLine("");
                sb.AppendLine("");
                sb.AppendLine(
                    "//------------------------------Abstract Specification Model------------------------------");
                sb.AppendLine("");
                sb.AppendLine("//shared variable");
                sb.AppendLine("var sur = 0;");
                sb.AppendLine("var ind= false;");
                sb.AppendLine("");
                sb.AppendLine("");
                sb.AppendLine("AA(i) = ai.i -> tau{sur = sur + 1; ind= true;}-> ar.i -> Skip;");
                sb.AppendLine("");
                sb.AppendLine("DA(i) = di.i -> tau{sur = sur- 1;if (sur == 0) {ind= false;}}-> dr.i -> Skip; ");
                sb.AppendLine("");
                sb.AppendLine("PA(i, j) = [j <2](AA(i);DA(i);PA(i, j+1));");
                sb.AppendLine("");
                sb.AppendLine("QA() = q.ind-> QA();");
                sb.AppendLine("");
                sb.AppendLine("SNZIAbs() = (|||x:{0..P-1}@PA(x, 0)) ||| QA();");
                sb.AppendLine("");
                sb.AppendLine("");
                sb.AppendLine("#assert SNZI() refines SNZIAbs();");
                sb.AppendLine("#assert SNZIAbs() refines SNZI();");

                return sb.ToString();
            }

            sb.AppendLine("//This model is for \"SNZI: Scalable NonZero Indicators\"");
            sb.AppendLine("");
            sb.AppendLine("//number of processes");
            sb.AppendLine("#define PRO_SIZE 2;");
            sb.AppendLine("");
            sb.AppendLine("// number of nodes");
            sb.AppendLine("#define NODE_SIZE 2;");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("//------------------------------shared variable------------------------------");
            sb.AppendLine(
                "//Since SNZI algorithm is organized as a rooted tree of SNZI objects, we create a NODE_SIZE array of Node objects.");
            sb.AppendLine(
                "//The root is the first element of the array,  and for 0 < i < NODE_SIZE, the parent of the ith node is the (i-1)/2th node.");
            sb.AppendLine("");
            sb.AppendLine("//array of shared variables of nodes (including hierarchical and root nodes), ");
            sb.AppendLine("//each nodes has its own local copy. so there are NODE_SIZE copies ");
            sb.AppendLine("var node_c[NODE_SIZE];  //the original X.c");
            sb.AppendLine("var node_v[NODE_SIZE];  //the original X.v");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("var node_a = 0;    //the origina X.a for the root, i.e. the first element of the array");
            sb.AppendLine("");
            sb.AppendLine("//presence indicator");
            sb.AppendLine("var I = false;");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("//------------------------------local variable------------------------------");
            sb.AppendLine("");
            sb.AppendLine(
                "//array of local variables which are used in the corresponding operations of nodes when a process arrives at or departs from nodes, ");
            sb.AppendLine("// i.e. representing x in the original algorithm");
            sb.AppendLine(
                "//as there may be N processes visiting one node concurrently, there could be NODE_SIZE * PRO_SIZE local variables");
            sb.AppendLine("//this is a variant of 2-dimention array. for node i, the local variable of process j");
            sb.AppendLine("//can be calculated by (i * PRO_SIZE + j)");
            sb.AppendLine("var cc[NODE_SIZE * PRO_SIZE];");
            sb.AppendLine("var vv[NODE_SIZE * PRO_SIZE];");
            sb.AppendLine("");
            sb.AppendLine(
                "//aa is the local variable of root node, so there will at most PRO_SIZE copies because only at most");
            sb.AppendLine("//PRO_SIZE processes can visit root node at the same time.");
            sb.AppendLine("var aa[PRO_SIZE];");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("//another local variables of root node, representing x' in the original algorithm");
            sb.AppendLine("//As above, only at most PRO_SIZE processes can visit the root node concurrently,");
            sb.AppendLine("//so each array contains PRO_SIZE elements.");
            sb.AppendLine("var rootc[PRO_SIZE];");
            sb.AppendLine("var roota[PRO_SIZE];");
            sb.AppendLine("var rootv[PRO_SIZE];");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("//other local variables of hierarchical SNZI node");
            sb.AppendLine("//for each such node, PRO_SIZE processes can visit it simultoneously, ");
            sb.AppendLine("//so the total number of each local varaible should be NODE_SIZE * PRO_SIZE");
            sb.AppendLine("var succ[NODE_SIZE * PRO_SIZE];    //the original succ in the Arrive operation ");
            sb.AppendLine("var undoArr[NODE_SIZE * PRO_SIZE]; //the original undoArr in the Arrive operation");
            sb.AppendLine("");
            sb.AppendLine("//for LL-SC primitive");
            sb.AppendLine("var updateCounter;");
            sb.AppendLine("var pro_counter[PRO_SIZE];");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine(
                "//------------------------------The Concrete Implementation Model------------------------------");
            sb.AppendLine("//Single Entry of Arrival and Departure operations on any nodes");
            sb.AppendLine(
                "ArriveImpl(process, node) = arrive_inv.process -> ArriveGeneral(process,node); arrive_res.process -> Skip;");
            sb.AppendLine("");
            sb.AppendLine(
                "DepartImpl(process, node) = depart_inv.process -> DepartGeneral(process,node);depart_res.process -> Skip;");
            sb.AppendLine("");
            sb.AppendLine(
                "ArriveGeneral(process, node) = ifa (node == 0) {ArriveRoot(process)} else { Arrive(process, node)};");
            sb.AppendLine("");
            sb.AppendLine(
                "DepartGeneral(process,node) = ifa (node == 0) {DepartRoot(process)} else {Depart(process,node)};");
            sb.AppendLine("");
            sb.AppendLine(
                "//------------------------------start - this part is for root node------------------------------");
            sb.AppendLine("//Arrival on root node");
            sb.AppendLine(
                "ArriveRoot(process) =  t {cc[process] = node_c[0]; aa[process] = node_a; vv[process] = node_v[0];} // x <- Read(X)");
            sb.AppendLine("					  -> Repeat(process);Until(process);");
            sb.AppendLine("");
            sb.AppendLine("Repeat(process) = if (cc[process] == 0) ");
            sb.AppendLine("				 {   // if x.c = 0 ");
            sb.AppendLine(
                "					 t {rootc[process] = 1; roota[process] = true; rootv[process] = vv[process] + 1;} -> Skip //  x'<- (1, true, x.v + 1)");
            sb.AppendLine("				 }		");
            sb.AppendLine("				else ");
            sb.AppendLine("				{");
            sb.AppendLine(
                "					 t {rootc[process] = cc[process] + 1; roota[process] = aa[process]; rootv[process] = vv[process];} -> Skip // x'<-(x.c+1, x.a, x.v)");
            sb.AppendLine("					");
            sb.AppendLine("				};");
            sb.AppendLine(
                "Until(process) = ifa (cc[process] == node_c[0]&& aa[process] == node_a && vv[process] == node_v[0])  //  until CAS(X, x, x')");
            sb.AppendLine("				{           ");
            sb.AppendLine(
                "					t {node_c[0] = rootc[process]; node_a = roota[process]; node_v[0] = rootv[process];} -> Write(process)   ");
            sb.AppendLine("				}");
            sb.AppendLine("				else {");
            sb.AppendLine("					ArriveRoot(process)");
            sb.AppendLine("				};");
            sb.AppendLine("");
            sb.AppendLine("Write(process) = if (roota[process] == true) {// if x'.a then");
            sb.AppendLine("				 	t {I = true; updateCounter = updateCounter + 1;} -> CAS(process)   // Write(I, true)");
            sb.AppendLine("				}");
            sb.AppendLine("				else {");
            sb.AppendLine("					 Skip");
            sb.AppendLine("				};");
            sb.AppendLine("		");
            sb.AppendLine(
                "CAS(process) = ifa (node_c[0] == rootc[process] && node_a == roota[process] && node_v[0] == rootv[process]) { //CAS(X,x',(x'.c, false, x'.v))");
            sb.AppendLine("					t {node_c[0] = rootc[process]; node_a = false; node_v[0] = rootv[process];} -> Skip");
            sb.AppendLine("				}");
            sb.AppendLine("				else {");
            sb.AppendLine("					Skip");
            sb.AppendLine("				};");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("//Departure from root node");
            sb.AppendLine(
                "DepartRoot(process) =  t {cc[process] = node_c[0]; aa[process] = node_a; vv[process] = node_v[0];}-> line15(process);  ");
            sb.AppendLine(
                "line15(process) = ifa (cc[process] == node_c[0] && aa[process] == node_a && vv[process] == node_v[0]) //if CAS(X, x, (x.c - 1, false, x.v))");
            sb.AppendLine("				  { ");
            sb.AppendLine(
                "					t {node_c[0] = cc[process] - 1; node_a = false; node_v[0] = vv[process];} -> l151(process)");
            sb.AppendLine("				  }");
            sb.AppendLine("				else { DepartRoot(process)};");
            sb.AppendLine("				");
            sb.AppendLine("l151(process) = if (cc[process] > 1) {Skip}  ");
            sb.AppendLine(" 				else {DepartRootLoop(process) };");
            sb.AppendLine("																");
            sb.AppendLine("DepartRootLoop(process) = t { pro_counter[process] = updateCounter;} // LL(I)");
            sb.AppendLine("						-> ");
            sb.AppendLine("						if (vv[process] != node_v[0]) { Skip}");
            sb.AppendLine("						else");
            sb.AppendLine("						{");
            sb.AppendLine("							ifa ( pro_counter[process] != updateCounter) {t -> DepartRootLoop(process)}");
            sb.AppendLine("							else {");
            sb.AppendLine("							   t {I = false; updateCounter = updateCounter + 1;} -> Skip");
            sb.AppendLine("							}");
            sb.AppendLine("						}");
            sb.AppendLine("		             ;");
            sb.AppendLine(
                "//------------------------------end - this part is for root node------------------------------");
            sb.AppendLine("");
            sb.AppendLine("		");
            sb.AppendLine("		");
            sb.AppendLine("		");
            sb.AppendLine("//------------------------------start - hierarchical SNZI node------------------------------");
            sb.AppendLine("//Arrival of hierarchical SNZI node");
            sb.AppendLine("Arrive(process, node) = t {succ[node * PRO_SIZE + process] = false;} ");
            sb.AppendLine("						-> t {undoArr[node * PRO_SIZE + process] = 0;} ");
            sb.AppendLine("						-> ArriveLoop1(process, node); ArriveLoop2(process, node); Skip;");
            sb.AppendLine("");
            sb.AppendLine("ArriveLoop1(process, node) = if (succ[node * PRO_SIZE + process] == false) {");
            sb.AppendLine("							");
            sb.AppendLine(
                "								t {cc[node * PRO_SIZE + process] = node_c[node]; vv[node * PRO_SIZE + process] = node_v[node];}");
            sb.AppendLine("								-> ");
            sb.AppendLine("								ArriveCase1(process, node)");
            sb.AppendLine("			                }else {Skip};");
            sb.AppendLine("			                ");
            sb.AppendLine(
                " //Because 1/2 cannot be expressed in pat, all values of x.c and X.c on hierarchical node will be twice as the original values.           ");
            sb.AppendLine("ArriveCase1(process, node) = if (cc[node * PRO_SIZE + process] > 1 ) { //if x.c >= 1 then");
            sb.AppendLine("							 l2(process, node);ArriveCase2(process, node)");
            sb.AppendLine("							 }");
            sb.AppendLine("							 else {");
            sb.AppendLine("							    ArriveCase2(process, node)");
            sb.AppendLine("							 };");
            sb.AppendLine("	");
            sb.AppendLine(
                "l2(process, node) = ifa (cc[node * PRO_SIZE + process] == node_c[node] && vv[node * PRO_SIZE + process] == node_v[node]) { //if CAS(X, x, (x.c + 1, x.v)) then ");
            sb.AppendLine(
                "						t {node_c[node] = cc[node * PRO_SIZE + process] + 2; node_v[node] = vv[node * PRO_SIZE + process];} ");
            sb.AppendLine("						-> ");
            sb.AppendLine("						t {succ[node * PRO_SIZE + process] = true;} -> Skip");
            sb.AppendLine("					}");
            sb.AppendLine("					else { t -> Skip};");
            sb.AppendLine("		");
            sb.AppendLine("ArriveCase2(process, node) = if (cc[node * PRO_SIZE + process]== 0) { // if x.c = 0 then");
            sb.AppendLine("								l3(process, node);ArriveCase3(process, node)");
            sb.AppendLine("							}");
            sb.AppendLine("							else {");
            sb.AppendLine("							  ArriveCase3(process, node)");
            sb.AppendLine("							};");
            sb.AppendLine("");
            sb.AppendLine(
                "l3(process, node) =  ifa (cc[node * PRO_SIZE + process] == node_c[node] && vv[node * PRO_SIZE + process] == node_v[node]) { // if CAS(X, x, (1 / 2, x.v + 1)) then");
            sb.AppendLine("						t {node_c[node] = 1; node_v[node] = vv[node * PRO_SIZE + process] + 1;} ");
            sb.AppendLine("						-> ");
            sb.AppendLine("						t {succ[node * PRO_SIZE + process] = true;}");
            sb.AppendLine("						->");
            sb.AppendLine(
                "						t{cc[node * PRO_SIZE + process] = 1; vv[node * PRO_SIZE + process] = vv[node * PRO_SIZE + process] + 1;} ");
            sb.AppendLine("						-> Skip");
            sb.AppendLine("					}");
            sb.AppendLine("					else { t -> Skip};");
            sb.AppendLine("");
            sb.AppendLine("//if x.c = 1/2 then");
            sb.AppendLine("ArriveCase3(process, node) = if (cc[node * PRO_SIZE + process] == 1) {  ");
            sb.AppendLine("								ArriveGeneral(process, (node - 1)/2);l5(process,node)");
            sb.AppendLine("							}");
            sb.AppendLine("							else");
            sb.AppendLine("							{");
            sb.AppendLine("							   ArriveLoop1(process, node)");
            sb.AppendLine("							};");
            sb.AppendLine("");
            sb.AppendLine(
                "l5(process, node) = ifa (cc[node * PRO_SIZE + process] == node_c[node] && vv[node * PRO_SIZE + process] == node_v[node]) {");
            sb.AppendLine(
                "						t{node_c[node] = 2; node_v[node] = vv[node * PRO_SIZE + process];} -> ArriveLoop1(process, node)");
            sb.AppendLine("					}");
            sb.AppendLine("					else {");
            sb.AppendLine(
                "						t -> t{undoArr[node * PRO_SIZE + process] = undoArr[node * PRO_SIZE + process] + 1;} -> ArriveLoop1(process, node)");
            sb.AppendLine("					};");
            sb.AppendLine("		 ");
            sb.AppendLine("	 		 ");
            sb.AppendLine("ArriveLoop2(process, node) = if (undoArr[node * PRO_SIZE + process] > 0) {");
            sb.AppendLine("							    DepartGeneral(process, (node - 1)/2) ;");
            sb.AppendLine("								t{undoArr[node * PRO_SIZE + process] = undoArr[node * PRO_SIZE + process] - 1;} ");
            sb.AppendLine("								->ArriveLoop2(process, node)");
            sb.AppendLine("							}");
            sb.AppendLine("				           else { Skip};");
            sb.AppendLine("");
            sb.AppendLine("//Departure of hierarchical SNZI node");
            sb.AppendLine(
                "Depart(process, node) =  t {cc[node * PRO_SIZE + process] = node_c[node]; vv[node * PRO_SIZE + process] = node_v[node];} ");
            sb.AppendLine("						-> l8(process, node); Skip;");
            sb.AppendLine("						");
            sb.AppendLine(
                "l8(process, node) = ifa (cc[node * PRO_SIZE + process] == node_c[node] && vv[node * PRO_SIZE + process] == node_v[node]) { //if CAS(X, x, (x.c - 1, x.v)) then");
            sb.AppendLine(
                "						t{node_c[node] = cc[node * PRO_SIZE + process] - 2; node_v[node] = vv[node * PRO_SIZE + process];} ");
            sb.AppendLine("						-> ");
            sb.AppendLine("						l9(process, node)");
            sb.AppendLine("					}");
            sb.AppendLine("					else {t -> Depart(process, node)};");
            sb.AppendLine("");
            sb.AppendLine("// if x.c = 1 then call parent.Depart		");
            sb.AppendLine("l9(process, node) = if (cc[node * PRO_SIZE + process] == 2) {");
            sb.AppendLine("						DepartGeneral(process, (node - 1)/2); 	  Skip");
            sb.AppendLine("					}");
            sb.AppendLine("					else {");
            sb.AppendLine("						Skip");
            sb.AppendLine("					};");
            sb.AppendLine("//------------------------------end - hierarchical SNZI node------------------------------");
            sb.AppendLine("");
            sb.AppendLine("		");
            sb.AppendLine("		");
            sb.AppendLine(
                "Process(i, j) = [j <2]([] x:{0..NODE_SIZE - 1}@(ArriveImpl(i, x);DepartImpl(i, x));Process(i, j+1)); ");
            sb.AppendLine("");
            sb.AppendLine("Query() = query.I -> Query();");
            sb.AppendLine("");
            sb.AppendLine("SNZI() = (|||x:{0..PRO_SIZE - 1}@Process(x, 0)) \\{t} ||| Query();");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("//------------------------------Abstract Specification Model------------------------------");
            sb.AppendLine("");
            sb.AppendLine("//shared variable");
            sb.AppendLine("var surplus = 0;");
            sb.AppendLine("var indicator = false;");
            sb.AppendLine("");
            sb.AppendLine("ArriveAbs(i) = arrive_inv.i -> t{surplus = surplus + 1; indicator = 1;}-> arrive_res.i -> Skip;");
            sb.AppendLine("");
            sb.AppendLine("DepartAbs(i) = depart_inv.i -> t{surplus = surplus - 1;");
            sb.AppendLine("				if (surplus == 0) {indicator = false;} }");
            sb.AppendLine("				-> depart_res.i -> Skip; ");
            sb.AppendLine("");
            sb.AppendLine("ProcessAbs(i, j) = [j <2](ArriveAbs(i);DepartAbs(i);ProcessAbs(i, j+1));");
            sb.AppendLine("");
            sb.AppendLine("QueryAbs() = query.indicator -> QueryAbs();");
            sb.AppendLine("");
            sb.AppendLine("SNZIAbs() = (|||x:{0..PRO_SIZE-1}@ProcessAbs(x, 0)) \\ {t}||| QueryAbs();");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("#assert SNZI() refines SNZIAbs();");
            sb.AppendLine("#assert SNZIAbs() refines SNZI();");

            return sb.ToString();

        }

        public static string LoadQueueImplementation(int N, int queueSize, bool isCorrect)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("//number of processes");
            sb.AppendLine("#define N " + N + ";");
            sb.AppendLine("//queue size");
            sb.AppendLine("#define SIZE " + queueSize + ";");
            sb.AppendLine();

            sb.AppendLine("//shared front pointer for the concrete implementation");
            sb.AppendLine("var F = 0;");
            sb.AppendLine("//shared rear pointer for the concrete implementation");
            sb.AppendLine("var R = 0;");
            sb.AppendLine("//shared Queue for the concrete implementation");
            sb.AppendLine("var Q[SIZE+1];");

            sb.AppendLine();
            sb.AppendLine("//local variable to store the temporary front/rear/queue value");
            sb.AppendLine("var FL[N];");
            sb.AppendLine("var RL[N];");
            sb.AppendLine("var QL[N];");
            sb.AppendLine();

            sb.AppendLine("//shared front/rear pointer for the abstract implemetation");
            sb.AppendLine("var FA = 0;");
            sb.AppendLine("var RA = 0;");
            sb.AppendLine("//local variable to store the size of the queue");
            sb.AppendLine("var SL[N];");
            sb.AppendLine();

            string s = "";
            sb.AppendLine("////////////////The Concrete Implementation Model//////////////////");
            sb.AppendLine("Enq(i) = enq_inv.i -> EnqLoop(i);");
            sb.AppendLine("EnqLoop(i) = t{RL[i] = R;} -> t{QL[i] = Q[RL[i]];} ->");
            sb.AppendLine("\t (EnqLoop(i) <<RL[i]==R>> ");
            sb.AppendLine("\t\t (EnqLoop(i) <<!(F - RL[i]==1 || RL[i] == F + SIZE)>>");
            if (isCorrect)
            {
                sb.AppendLine("\t\t\t ( ((EnqLoop(i) << Q[RL[i]] != 0 >> (EnqLoop(i) <<RL[i]==R>> t{R = (R+1)%(SIZE+1);}-> EnqLoop(i)))");
            }
            else
            {
                sb.AppendLine("\t\t\t ( (EnqLoop(i) <<RL[i]==R>> t{R = (R+1)%(SIZE+1);}-> EnqLoop(i))");
            }
            sb.AppendLine("\t\t\t\t <<QL[i] == 0>>");
            sb.AppendLine("\t\t\t\t\t (EnqLoop(i) <<Q[ RL[i] ] == QL[i]>>");
            sb.AppendLine("\t\t\t\t\t\t t{");
            sb.AppendLine("\t\t\t\t\t\t\t Q[RL[i]] = 1;");
            sb.AppendLine("\t\t\t\t\t\t\t if(R >= F) {");
            sb.AppendLine("\t\t\t\t\t\t\t\t FL[i] = R - F + 1;");
            sb.AppendLine("\t\t\t\t\t\t\t } else {");
            sb.AppendLine("\t\t\t\t\t\t\t\t if(R < F - 1) {");
            sb.AppendLine("\t\t\t\t\t\t\t\t\t FL[i] = R + SIZE - F + 2;");
            sb.AppendLine("\t\t\t\t\t\t\t\t } else {");
            sb.AppendLine("\t\t\t\t\t\t\t\t\t FL[i] = SIZE;");
            sb.AppendLine("\t\t\t\t\t\t\t\t }");
            sb.AppendLine("\t\t\t\t\t\t\t }");
            sb.AppendLine("\t\t\t\t\t\t\t if(Q[ F ] == 0) {");
            sb.AppendLine("\t\t\t\t\t\t\t\t FL[i] = FL[i] - 1;");
            sb.AppendLine("\t\t\t\t\t\t\t }");
            sb.AppendLine("\t\t\t\t\t\t } -> (ResEnq(i) << Q[RL[i]] == 1 && RL[i]== R>> t{R = (R+1)%(SIZE+1);} -> ResEnq(i))");
            sb.AppendLine("\t\t\t\t\t )");
            sb.AppendLine("\t\t\t\t )");
            sb.AppendLine("\t\t\t )");
            sb.AppendLine("\t\t )");
            sb.AppendLine("\t);");

            sb.AppendLine();
            sb.Append("ResEnq(i) = ");

            for (int i = queueSize - 1; i > 0; i--)
            {
                if (i != queueSize - 1)
                {
                    s = "(" + s + " <<FL[i]==" + i + ">> (enq_res.i." + i + " -> Skip))";
                }
                else
                {
                    s = "(( enq_res.i." + (i + 1) + " -> Skip) <<FL[i]==" + i + ">> (enq_res.i." + i + " -> Skip))";
                }
            }
            sb.AppendLine(s + ";");
            sb.AppendLine();

            sb.AppendLine("Deq(i) = deq_inv.i -> DeqLoop(i);");
            sb.AppendLine("DeqLoop(i) = t{FL[i]= F;} -> t{QL[i] = Q[ FL[i] ];} ->");
            sb.AppendLine("\t (DeqLoop(i) <<FL[i]==F>> ");
            sb.AppendLine("\t\t (");
            if (isCorrect)
            {
                sb.AppendLine("\t\t\t ((DeqLoop(i) << Q[ FL[i] ] == 0 >> (DeqLoop(i) <<FL[i]==F>> t{F = (F+1)%(SIZE+1);}-> DeqLoop(i)))");
            }
            else
            {
                sb.AppendLine("\t\t\t t ->((DeqLoop(i) <<FL[i]==F>> t{F = (F+1)%(SIZE+1);}-> DeqLoop(i))");
            }
            sb.AppendLine("\t\t\t <<QL[i] != 0>> ");
            sb.AppendLine("\t\t\t\t t ->( DeqLoop(i)  << FL[i]==F && Q[ FL[i] ] == QL[i] >>");
            sb.AppendLine("\t\t\t\t\t   t{");
            sb.AppendLine("\t\t\t\t\t\t   Q[ FL[i] ]= 0;");
            sb.AppendLine("\t\t\t\t\t\t   if(R > F) {");
            sb.AppendLine("\t\t\t\t\t\t\t   RL[i] = R - F ;");
            sb.AppendLine("\t\t\t\t\t\t   }else {");
            sb.AppendLine("\t\t\t\t\t\t\t   RL[i] = R + SIZE - F + 1;");
            sb.AppendLine("\t\t\t\t\t\t   };  ");
            sb.AppendLine("\t\t\t\t\t\t   if(Q[ R ] == 1) {");
            sb.AppendLine("\t\t\t\t\t\t\t   RL[i] =  RL[i]  + 1;");
            sb.AppendLine("\t\t\t\t\t\t   }");
            sb.AppendLine("\t\t\t\t\t   } -> (ResDeq(i) <<FL[i]==F>> t{F = (F+1)%(SIZE+1);} -> ResDeq(i))");
            sb.AppendLine("\t\t\t\t )");
            sb.AppendLine("\t\t   )");
            sb.AppendLine("\t\t   << FL[i] == R >> deq_res.i.0 -> Skip");
            sb.AppendLine("\t\t )");
            sb.AppendLine("\t );");


            sb.Append("ResDeq(i) = ");

            for (int i = queueSize - 1; i > 0; i--)
            {
                if (i != queueSize - 1)
                {
                    s = "(" + s + " <<RL[i]==" + i + ">> (deq_res.i." + i + " -> Skip))";
                }
                else
                {
                    s = "(( deq_res.i." + (i + 1) + " -> Skip) <<RL[i]==" + i + ">> (deq_res.i." + i + " -> Skip))";
                }
            }
            sb.AppendLine(s + ";");
            sb.AppendLine();

            sb.AppendLine("Process(i) = (Enq(i)[]Deq(i));Process(i);");
            sb.AppendLine("Queue() = (|||x:{0..N-1}@Process(x)) \\ {t};");
            sb.AppendLine();

            sb.AppendLine("////////////////The Abstract Specification Model//////////////////");
            sb.AppendLine("EnqAbs(i) = enq_inv.i -> ");
            sb.AppendLine("\t t{  if(RA >= FA && RA - FA < SIZE) {  //if the Rear pointer is after the Front pointer");
            sb.AppendLine("\t\t SL[i]= RA - FA + 1;");
            sb.AppendLine("\t\t RA = (RA+1)%(SIZE+1);");
            sb.AppendLine("\t\t } else {                            //else the Rear pointer is in front of Front pointer");
            sb.AppendLine("\t\t\t if(RA < FA - 1) {");
            sb.AppendLine("\t\t\t\t RA = RA+1;");
            sb.AppendLine("\t\t\t\t SL[i] = RA + SIZE - FA + 1;");
            sb.AppendLine("\t\t\t } else {                          //else the queue is full");
            sb.AppendLine("\t\t\t\t SL[i] = SIZE;");
            sb.AppendLine("\t\t\t }");
            sb.AppendLine("\t\t }");
            sb.AppendLine("\t } ->");

            for (int i = queueSize - 1; i > 0; i--)
            {
                if (i != queueSize - 1)
                {
                    s = "(" + s + " <<SL[i]==" + i + ">> (enq_res.i." + i + " -> Skip))";
                }
                else
                {
                    s = "(( enq_res.i." + (i + 1) + " -> Skip) <<SL[i]==" + i + ">> (enq_res.i." + i + " -> Skip))";
                }
            }
            sb.AppendLine(s + ";");
            sb.AppendLine();

            sb.AppendLine("DeqAbs(i) = deq_inv.i ->");
            sb.AppendLine("\t ((t-> deq_res.i.0 -> Skip) <<FA != RA>> ");
            sb.AppendLine("\t\t (t{");
            sb.AppendLine("\t\t\t if(RA >= FA) {    //if the Rear pointer is after the Front pointer");
            sb.AppendLine("\t\t\t\t SL[i] = RA - FA;");
            sb.AppendLine("\t\t\t }else {           //else the Rear pointer is in front of Front pointer");
            sb.AppendLine("\t\t\t\t SL[i] = RA + SIZE -FA + 1;");
            sb.AppendLine("\t\t\t }");
            sb.AppendLine("\t\t\t FA = (FA + 1)% (SIZE +1);  //update the Front pointer");
            sb.Append("\t\t } ->");

            s = "";
            for (int i = queueSize - 1; i > 0; i--)
            {
                if (i != queueSize - 1)
                {
                    s = "(" + s + " <<SL[i]==" + i + ">> (deq_res.i." + i + " -> Skip))";
                }
                else
                {
                    s = "(( deq_res.i." + (i + 1) + " -> Skip) <<SL[i]==" + i + ">> (deq_res.i." + i + " -> Skip))";
                }
            }
            sb.AppendLine(s);
            sb.AppendLine("\t\t )");
            sb.AppendLine("\t );");

            sb.AppendLine();
            sb.AppendLine("ProcessAbs(i) = (EnqAbs(i)[]DeqAbs(i));ProcessAbs(i);");

            sb.AppendLine();
            sb.Append("QueueAbs() = (|||x:{0..N-1}@ProcessAbs(x)) \\ {t};");

            sb.AppendLine();

            sb.AppendLine("////////////////The Properties//////////////////");
            sb.AppendLine("#assert Queue() refines QueueAbs();");
            sb.AppendLine("#assert QueueAbs() refines Queue();");
            return sb.ToString();
        }


        public static string LoadMailboxProblem(int N, bool isblocking, bool isOptimized)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("#import \"PAT.Lib.Example\";");
            sb.AppendLine(); ;
            sb.AppendLine("#define EQ 0;");
            sb.AppendLine("#define NEQ 1;");
            sb.AppendLine(" ");
            sb.AppendLine("#define BOTTOM  0;");
            sb.AppendLine("#define UNKNOWN  1;");
            sb.AppendLine("#define SUCCESS  2;");
            sb.AppendLine("#define DONE  3;");
            sb.AppendLine(" ");
            sb.AppendLine("//bounded number of Rounds");
            sb.AppendLine("#define ROUND " + (2 * N) + ";");
            sb.AppendLine("#define MaxROUND " + N + ";");
            sb.AppendLine(" ");
            sb.AppendLine("var A0TS[ROUND];");
            sb.AppendLine("var A0C[ROUND];");
            sb.AppendLine(" ");
            sb.AppendLine("var A1TS[ROUND];");
            sb.AppendLine("var A1C[ROUND];");
            sb.AppendLine(" ");
            sb.AppendLine("var B0[ROUND];");
            sb.AppendLine("var B1[ROUND];");
            sb.AppendLine(" ");
            sb.AppendLine("var TS = [1, 1];");
            sb.AppendLine("var Rel = [0, 0];");
            sb.AppendLine(" ");
            sb.AppendLine("var Counter[2];");
            sb.AppendLine("var Rounds[2];");
            sb.AppendLine("var Otherc[2];");
            sb.AppendLine("var OutvalueTS[2];");
            sb.AppendLine("var OutvalueC[2];");
            sb.AppendLine("var Outcome[2];");
            sb.AppendLine("var TSL[2];");
            sb.AppendLine("var RelL[2];");
            sb.AppendLine(" ");
            sb.AppendLine("var NextTS=[2,2];");
            sb.AppendLine("var Otherts = [1,1];");
            sb.AppendLine("var TSC = [1, 1];");
            sb.AppendLine(" ");
            sb.AppendLine("var rnd0 = 0;");
            sb.AppendLine("var rnd1 = 0;");
            sb.AppendLine(" ");
            sb.AppendLine("////////////////The Concrete Implementation Model//////////////////");
            sb.AppendLine("//process 0: postman");
            sb.AppendLine("Postman(i) = [i < MaxROUND](deliver_inv -> tau{Counter[0] = Counter[0] + 1;} -> (Compare(0); deliver_res -> Postman(i+1)));");
            sb.AppendLine(" ");
            sb.AppendLine("Compare(i) = tau{Outcome[i] = UNKNOWN;} -> ");
            sb.AppendLine("   (CompareLoop(i); ");
            sb.AppendLine("    (if(Counter[i]!=Otherc[i]) {");
            sb.AppendLine("         tau{TS[i] = TSC[i]; Rel[i] = NEQ;} -> Skip");
            sb.AppendLine("     } else { ");
            sb.AppendLine("         tau{TS[i] = TSC[i]; Rel[i] = EQ;} -> Skip");
            sb.AppendLine("     })");
            sb.AppendLine("   );");
            sb.AppendLine(" ");

            string updateRound = "";
            if (isOptimized)
            {
                sb.AppendLine("var TRound[2];");
                sb.AppendLine("var TOtherRound[2];");
                updateRound = "tau{TOtherRound[i] = Rounds[1-i];} -> tau{ if(Rounds[i]+1 > TOtherRound[i]-1) { TRound[i] = Rounds[i]+1;} else {TRound[i] = TOtherRound[i]-1;} } -> tau{Rounds[i] = TRound[i];} -> ";
            }
            else
            {
                updateRound = "tau{Rounds[i] = Rounds[i]+1;} -> ";
            }
            
            sb.AppendLine("CompareLoop(i)  =  if (Outcome[i] == SUCCESS) { Skip } else {");
            if (!isblocking)
            {
                sb.AppendLine("              if(i == 1 && Counter[i] < Otherc[i]) { ");
                sb.AppendLine("                  Skip ");
                sb.AppendLine("              } else {");
                sb.AppendLine("                  " + updateRound + "tau{TSC[i] = NextTS[i];} -> Sussus(i);");
                sb.AppendLine("                        (if (OutvalueC[i] !=  BOTTOM) { ");
                sb.AppendLine("                             tau{Otherts[i] = OutvalueTS[i];} -> tau{Otherc[i] = OutvalueC[i];} -> Skip");
                sb.AppendLine("                        	} else {");
                sb.AppendLine("                             Skip");
                sb.AppendLine("                         });");
                sb.AppendLine("                         tau{NextTS[i] = call(dominate, Otherts[i], TS[1-i]);} -> CompareLoop(i)");
                sb.AppendLine("              }};");
            }
            else
            {
                sb.AppendLine("                  " + updateRound + "tau{TSC[i] = NextTS[i];} -> Sussus(i);");
                sb.AppendLine("                        (if(OutvalueC[i] !=  BOTTOM) { ");
                sb.AppendLine("                             tau{Otherts[i] = OutvalueTS[i];} -> tau{Otherc[i] = OutvalueC[i];} -> Skip");
                sb.AppendLine("                        } else {");
                sb.AppendLine("                             Skip");
                sb.AppendLine("                        });");
                sb.AppendLine("                        tau{NextTS[i] = call(dominate, Otherts[i], TS[1-i]);} -> CompareLoop(i)");
                sb.AppendLine("                  };");
            }

            sb.AppendLine(" ");
            sb.AppendLine("Sussus(i) = if(i==0) { Sussus0() } else { Sussus1()};");
            sb.AppendLine(" ");
            sb.AppendLine("Sussus0() = tau{rnd0 = Rounds[0] - 1; A0TS[rnd0] = TSC[0]; A0C[rnd0] = Counter[0];} ->  tau{OutvalueTS[0] = A1TS[rnd0]; OutvalueC[0] = A1C[rnd0];} ->");
            sb.AppendLine("    (if(OutvalueC[0] == BOTTOM ) {");
            sb.AppendLine("        tau{Outcome[0] = SUCCESS;} -> Skip");
            sb.AppendLine("    } else {");
            sb.AppendLine("        tau{B0[rnd0] = DONE;} ->");
            sb.AppendLine("            (if(B1[rnd0] == BOTTOM) {");
            sb.AppendLine("                tau{Outcome[0] = UNKNOWN;} -> Skip");
            sb.AppendLine("             } else {");
            sb.AppendLine("                tau{Outcome[0] = SUCCESS;} -> Skip");
            sb.AppendLine("             })");
            sb.AppendLine("    });");
            sb.AppendLine(" ");

            sb.AppendLine("Sussus1() = tau{rnd1 = Rounds[1] - 1; A1TS[rnd1] = TSC[1]; A1C[rnd1] = Counter[1];} ->  tau{OutvalueTS[1] = A0TS[rnd1]; OutvalueC[1] = A0C[rnd1];} ->");
            sb.AppendLine("    (if(OutvalueTS[1] == BOTTOM ) {");
            sb.AppendLine("        tau{Outcome[1] = SUCCESS;} -> Skip");
            sb.AppendLine("    } else {");
            sb.AppendLine("        tau{B1[rnd1] = DONE;} ->");
            sb.AppendLine("            (if(B0[rnd1] == BOTTOM) {");
            sb.AppendLine("                tau{Outcome[1] = UNKNOWN;} -> Skip");
            sb.AppendLine("             } else {");
            sb.AppendLine("                tau{Outcome[1] = SUCCESS;} -> Skip");
            sb.AppendLine("             })");
            sb.AppendLine("    });");
            sb.AppendLine(" ");
            sb.AppendLine("//process 1: wife");
            sb.AppendLine("Check() = check_inv -> tau{TSL[0] = TS[0]; RelL[0] = Rel[0];} -> tau{TSL[1] = TS[1]; RelL[1] = Rel[1];} ->");
            sb.AppendLine("   (if(call(mailorder, TSL[0],  TSL[1])) {");
            sb.AppendLine("           (if(RelL[0] == EQ) {");
            sb.AppendLine("               check_resf -> Check()");
            sb.AppendLine("           } else {");
            sb.AppendLine("               check_rest -> Remove()");
            sb.AppendLine("           }) ");
            sb.AppendLine("    } else {");
            sb.AppendLine("           (if(RelL[1] == EQ) {");
            sb.AppendLine("               check_resf -> Check()");
            sb.AppendLine("           } else {");
            sb.AppendLine("               check_rest -> Remove()");
            sb.AppendLine("           })");
            sb.AppendLine("    }); ");
            sb.AppendLine("  ");
            sb.AppendLine("Remove() = remove_inv -> tau{Counter[1] = Counter[1] + 1;} -> (Compare(1); remove_res -> Skip);");

            sb.AppendLine("Wife(i) = [i < MaxROUND](Check();Wife(i+1));");
            sb.AppendLine("Mailbox() = Postman(0) ||| Wife(0);");
            sb.AppendLine(" ");
            sb.AppendLine("////////////////The Abstract Specification Model////////////////// ");
            sb.AppendLine("var FlagL = 0;");
            sb.AppendLine("var CountA = 0;");
            sb.AppendLine(" ");
            //sb.AppendLine("PostmanAbs() = deliver_inv -> deliver{CountA = CountA + 1;} -> deliver_res -> PostmanAbs();");
            //sb.AppendLine(" ");
            //sb.AppendLine("CheckAbs() = check_inv -> check{FlagL = CountA;} -> (if(FlagL > 0) {check_rest -> RemoveAbs()} else { check_resf -> CheckAbs() });");
            //sb.AppendLine("RemoveAbs() = remove_inv -> remove{CountA = CountA - 1;} -> remove_res -> Skip;");
            //sb.AppendLine("WifeAbs() = CheckAbs();WifeAbs();");
            //sb.AppendLine(" ");
            //sb.AppendLine("MailboxAbs() = (PostmanAbs() ||| WifeAbs()) \\{deliver, check, remove};");
            sb.AppendLine("PostmanAbs(i) =[i < MaxROUND]( deliver_inv -> deliver{CountA = CountA + 1;} -> deliver_res -> PostmanAbs(i+1));");
            sb.AppendLine(" ");
            sb.AppendLine("CheckAbs() = check_inv -> check{FlagL = CountA;} -> (if(FlagL > 0) {check_rest -> RemoveAbs()} else { check_resf -> CheckAbs() });");
            sb.AppendLine("RemoveAbs() = remove_inv -> remove{CountA = CountA - 1;} -> remove_res -> Skip;");
            sb.AppendLine("WifeAbs(i) = [i < MaxROUND] (CheckAbs();WifeAbs(i+1));");
            sb.AppendLine(" ");
            sb.AppendLine("MailboxAbs() = (PostmanAbs(0) ||| WifeAbs(0)) \\{deliver, check, remove};");
            sb.AppendLine(" ");
            sb.AppendLine("////////////////The Properties//////////////////");
            sb.AppendLine("#assert Mailbox() refines MailboxAbs();");
            sb.AppendLine("#assert MailboxAbs() refines Mailbox();");
            return sb.ToString();
        }


        public static string LoadLinkedList(int N, int nodes, int operationNumber)
        {
            StringBuilder sb = new StringBuilder();


            sb.AppendLine("#import \"PAT.Lib.EntryList\";");
            sb.AppendLine("#import \"PAT.Lib.Set\";");

            sb.AppendLine(" ");
            sb.AppendLine("//smalledst key for HEAD");
            sb.AppendLine("#define MIN 0;");
            sb.AppendLine("#define MIN1 1;");
            sb.AppendLine("");
            sb.AppendLine("//largest key for TAIL");
            sb.AppendLine("#define MAX 3; ");
            sb.AppendLine("#define MAX1 2;");
            sb.AppendLine("");
            sb.AppendLine("//number of processes");
            sb.AppendLine("#define N " + N + "; ");
            sb.AppendLine("");
            sb.AppendLine("//number of nodes including Head and Tail");
            sb.AppendLine("#define M " + nodes + ";");
            sb.AppendLine("");
            sb.AppendLine("//number of operations for each process");
            sb.AppendLine("#define Q " + operationNumber + ";");
            sb.AppendLine("");
            sb.AppendLine("//operation types");
            sb.AppendLine("enum {ADD, REMOVE, CONTAINS}; ");
            sb.AppendLine("");
            sb.AppendLine("//the underlying data structure to represent a singly linked list.");
            sb.AppendLine("var<EntryList> l = new EntryList(M, MIN, MAX);     ");
            sb.AppendLine("");
            sb.AppendLine("//array of local variables which are used in the operations, ");
            sb.AppendLine("//as there may be N processes calling one operation concurrently, there could be PROCESS_SIZE * OPERATION_SIZE local variables");
            sb.AppendLine("//For example, for add operation, the local variable curr of process j is c[j][0]");
            sb.AppendLine("//Note: variable[i][0] is local variable for ADD operation;");
            sb.AppendLine("//      variable[i][1] is local variable for REMOVE operation;");
            sb.AppendLine("//      variable[i][2] is local variable for CONTAINS operation;");
            sb.AppendLine("var c[N][3];           // N * 3");
            sb.AppendLine("var pred[N][3];        // N * 3");
            sb.AppendLine("var k[N][3];          // N * 3");
            sb.AppendLine("");
            sb.AppendLine("var entry[N];          // N * 1, for Add operation");
            sb.AppendLine("var r[N];              // N * 1, for Remove operation");
            sb.AppendLine("");
            sb.AppendLine("//var mp[N][2];");
            sb.AppendLine("var val[N][2];");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("//algorithm implementation");
            sb.AppendLine("//Note: in the implementaion, the garbage collection makes use of reference counting alg. Every assignment which involves the reference");
            sb.AppendLine("//      counter modification of the element in the linked list is encapsulated as a method call of Class EntryList in C# library. ");
            sb.AppendLine("//      So all gc details are a black box for this model. ");
            sb.AppendLine("");
            sb.AppendLine("Syst = (|||i:{0..N-1}@(Pro(i, 0)));");
            sb.AppendLine("         ");
            sb.AppendLine("Pro(i, j) = ifa(j < Q){(Add(i)[] Remove(i)[]Contains(i));Pro(i, j+1)};");
            sb.AppendLine("");
            sb.AppendLine("Add(i) =  []x:{MIN1..MAX1}@(add.i.x -> Loc(x,i,ADD));");
            sb.AppendLine("");
            sb.AppendLine("Remove(i) = []x:{MIN1..MAX1}@(rm.i.x -> Loc(x,i,REMOVE));");
            sb.AppendLine("");
            sb.AppendLine("Contains(i) = []x:{MIN1..MAX1}@(ct.i.x -> Loc(x,i,CONTAINS));");
            sb.AppendLine("");
            sb.AppendLine("Loc(key, p, o) = tau{pred[p][o]= 0} ->  //line 44: pred = Head");
            sb.AppendLine("                 tau{c[p][o]= l.GetNext(c[p][o], 0)}->  //line: 45curr = Head -> next");
            sb.AppendLine("                 l46(key, p, o);     ");
            sb.AppendLine("				 ");
            sb.AppendLine("l46(key, p, o) = if (l.Key(c[p][o])< key) //line 46: while(c-> key < key)");
            sb.AppendLine("                    {");
            sb.AppendLine("                       tau{l.Assign(pred[p][o],c[p][o]);pred[p][o]= c[p][o]; }-> //line 47: pred = curr");
            sb.AppendLine("                       tau{c[p][o]= l.GetNext(c[p][o], c[p][o])}->  //line48: c = curr ->next");
            sb.AppendLine("                       l46(key, p, o)");
            sb.AppendLine("                    }");
            sb.AppendLine("                    else");
            sb.AppendLine("                    {");
            sb.AppendLine("                      ifa(o ==ADD){Add5(key, p)}");
            sb.AppendLine("                      else  ");
            sb.AppendLine("                      {");
            sb.AppendLine("                      	ifa(o == REMOVE)  {Rm22(key, p)}");
            sb.AppendLine("                      	else {Ct38(key, p)}");
            sb.AppendLine("                      }");
            sb.AppendLine("                    };");
            sb.AppendLine("Add5(key, p)= ");
            sb.AppendLine("				tau{k[p][0] = (l.Key(c[p][0]) == key)} -> //line 5: k= (curr -> key == key)");
            sb.AppendLine("                if (k[p][0]== true) //line 6: if (k) return false");
            sb.AppendLine("                {");
            sb.AppendLine("                	add.p.key.(l.GetData()){");
            sb.AppendLine("									  //reset the local variables");
            sb.AppendLine("									  k[p][0]=false;");
            sb.AppendLine("									  	 val[p][0] = false;");
            sb.AppendLine("									  l.Reset3(pred[p][0], c[p][0], entry[p]);");
            sb.AppendLine("                	                  pred[p][0] = 0;");
            sb.AppendLine("                	                  c[p][0] = 0;");
            sb.AppendLine("                	                  entry[p] = 0;");
            sb.AppendLine("                	                  } -> Skip");
            sb.AppendLine("                }");
            sb.AppendLine("                else");
            sb.AppendLine("                { ");
            sb.AppendLine("                	//atomic {");
            sb.AppendLine("                	//	l7{entry[p] = l.Create(entry[p],key)} ->  //line 7: entry = new Entry(key)");
            sb.AppendLine("                	//	if (entry[p] == 0) {outOfMemory -> Stop} //not in original model, for checking memory leak");
            sb.AppendLine("                	//	else {l8{l.SetNext(entry[p], c[p][0])}-> Skip} //line 8: entry -> next = curr}");
            sb.AppendLine("                	//	l8{l.SetNext(entry[p], c[p][0])->Skip");
            sb.AppendLine("                	//}; ");
            sb.AppendLine("                    tau{entry[p] = l.Create(entry[p],key, c[p][0] );} -> //combine l7 and l8");
            sb.AppendLine("                    tau{");
            sb.AppendLine("                        //mp[p][0] = !(l.Marked(pred[p][0])); //line 10: mp = !pred -> marked");
            sb.AppendLine("                		val[p][0] = (l.Next(pred[p][0])== c[p][0]) && !(l.Marked(pred[p][0])); // line 11: val = (pred-> next == curr) && mp");
            sb.AppendLine("                		if (val[p][0] == true)// line 13: if (!val) goto restart");
            sb.AppendLine("                		{");
            sb.AppendLine("                		  l.SetNext(pred[p][0], entry[p]) //line 14: pred -> next = entry                		");
            sb.AppendLine("                		}");
            sb.AppendLine("                	} ->               	");
            sb.AppendLine("                	ifa(!val[p][0]) {Loc(key,p,ADD)}");
            sb.AppendLine("                	else{");
            sb.AppendLine("                		add.p.key.(l.GetData()){");
            sb.AppendLine("                		k[p][0]=false;");
            sb.AppendLine("                		 val[p][0] = false;");
            sb.AppendLine("                	 				l.Reset3(pred[p][0], c[p][0], entry[p]);");
            sb.AppendLine("                					pred[p][0] = 0;");
            sb.AppendLine("                					c[p][0] = 0;");
            sb.AppendLine("                	    			entry[p] = 0");
            sb.AppendLine("                		} -> Skip  ");
            sb.AppendLine("                	}");
            sb.AppendLine("                };");
            sb.AppendLine("");
            sb.AppendLine("Rm22(key, p)= ");
            sb.AppendLine("                tau{k[p][1] = (l.Key(c[p][1]) == key)} ->  //line 22:  k= (curr -> key == key)");
            sb.AppendLine("                if (!k[p][1])//line 23: if (!k) return false;");
            sb.AppendLine("                {");
            sb.AppendLine("                	rm.p.key.(l.GetData()){");
            sb.AppendLine("                	k[p][1]=false;");
            sb.AppendLine("                	              //    k[p][1] = false; ");
            sb.AppendLine("                	                  val[p][1] = false;");
            sb.AppendLine("                	                  l.Reset3(pred[p][1],c[p][1], r[p] );");
            sb.AppendLine("                	                  pred[p][1] = 0; ");
            sb.AppendLine("                	                  c[p][1] = 0;");
            sb.AppendLine("                	                  r[p] = 0;");
            sb.AppendLine("                	              } -> Skip");
            sb.AppendLine("                }");
            sb.AppendLine("                else");
            sb.AppendLine("                {");
            sb.AppendLine("                  tau{l.SetMarked(c[p][1], true)}-> //line 24: curr->marked = true");
            sb.AppendLine("                  tau{r[p] = l.GetNext(r[p], c[p][1])}-> //line 25: r = curr -> next                  ");
            sb.AppendLine("                  tau{");
            sb.AppendLine("                      //  mp[p][1] = !(l.Marked(pred[p][1])); //line 27: mp = !pred -> marked");
            sb.AppendLine("                		val[p][1] = (l.Next(pred[p][1])== c[p][1]) &&!(l.Marked(pred[p][1])); // line 28: val = (pred-> next == curr) && mp");
            sb.AppendLine("                		if (val[p][1]== true) //line 30: if(!val) goto restart");
            sb.AppendLine("                		{");
            sb.AppendLine("                		 l.SetNext(pred[p][1], r[p]) // line 31: pred-> next = r");
            sb.AppendLine("                		}");
            sb.AppendLine("                  }->                  ");
            sb.AppendLine("                  ifa (!val[p][1]) {Loc(key, p, REMOVE)}");
            sb.AppendLine("                  else{");
            sb.AppendLine("                  	rm.p.key.(l.GetData()){");
            sb.AppendLine("                	    k[p][1] = false; ");
            sb.AppendLine("                	 //   mp[p][1] = false;");
            sb.AppendLine("                	    val[p][1] = false;");
            sb.AppendLine("						l.Reset3(pred[p][1],c[p][1], r[p]);");
            sb.AppendLine("                		pred[p][1] = 0; ");
            sb.AppendLine("                		c[p][1] = 0; ");
            sb.AppendLine("                		r[p]= 0;");
            sb.AppendLine("                	}	-> Skip  ");
            sb.AppendLine("                 }");
            sb.AppendLine("                };");
            sb.AppendLine("                ");
            sb.AppendLine("Ct38(key, p) =  tau{k[p][2] = (l.Key(c[p][2]) == key)} -> //line 38: k= (curr -> key == key)");
            sb.AppendLine("                if (!k[p][2]) //line 39: if (!k) return false");
            sb.AppendLine("                {");
            sb.AppendLine("                	ct.p.key.(l.GetData()){");
            sb.AppendLine("                	                  k[p][2] = false; ");
            sb.AppendLine("                	                  l.Reset2(pred[p][2],c[p][2] );");
            sb.AppendLine("                	                  pred[p][2] = 0; ");
            sb.AppendLine("                	                  c[p][2] = 0");
            sb.AppendLine("                	                  } -> Skip");
            sb.AppendLine("                }");
            sb.AppendLine("                else");
            sb.AppendLine("                { ");
            sb.AppendLine("                  ct.p.key.(l.GetData()){");
            sb.AppendLine("               	                 k[p][2] = false; ");
            sb.AppendLine("                	             l.Reset2(pred[p][2],c[p][2] );");
            sb.AppendLine("                	             pred[p][2] = 0; ");
            sb.AppendLine("                	             c[p][2] = 0");
            sb.AppendLine("                	            } -> Skip");
            sb.AppendLine("                };");
            sb.AppendLine("");
            sb.AppendLine("//Specification");
            sb.AppendLine("var<Set> s;");
            sb.AppendLine("    ");
            sb.AppendLine("Sys = |||i:{0..N-1}@(P(i, 0));");
            sb.AppendLine("");
            sb.AppendLine("P(i, j) = ifa(j < Q){( (A(i, j)[] R(i, j)[]C(i, j)))};");
            sb.AppendLine("");
            sb.AppendLine("A(i, j) = []x:{MIN1..MAX1}@");
            sb.AppendLine("          ( ");
            sb.AppendLine("              add.i.x -> tau{s.Add(x)}-> add.i.x.(s.GetData())-> P(i, j+1) ");
            sb.AppendLine("          );");
            sb.AppendLine("");
            sb.AppendLine("R(i, j) = []x:{MIN1..MAX1}@");
            sb.AppendLine("          ( ");
            sb.AppendLine("              rm.i.x -> tau{s.Remove(x)}-> rm.i.x.(s.GetData())-> P(i, j+1) ");
            sb.AppendLine("          );");
            sb.AppendLine("C(i, j) = []x:{MIN1..MAX1}@");
            sb.AppendLine("          ( ");
            sb.AppendLine("              ct.i.x -> ct.i.x.(s.GetData())->  P(i, j+1) //{s.Contains(x)}");
            sb.AppendLine("          );");
            sb.AppendLine("");
            sb.AppendLine("//linearizibility Testing");
            sb.AppendLine("#assert Syst refines Sys;");
            //sb.AppendLine("#assert Syst deadlockfree;");

            return sb.ToString();
        }

        public static string LoadSieveofEratosthenes(int N)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("#define N " + N.ToString() + ";");
            int sqrt = (int)Math.Sqrt(N);
            sb.AppendLine("#define SqrtOfN " + sqrt.ToString() + ";");
            sb.AppendLine("var i=2;");
            sb.AppendLine("var j=2;");
            sb.AppendLine("var k=2;");
            sb.AppendLine();

            sb.AppendLine("//care the numbers from 2 to N");
            sb.AppendLine("//0 means not sieved, also prime");
            sb.AppendLine("//1 means already sieved, not prime");
            sb.AppendLine("var sievePrime[N+1];");
            sb.AppendLine();

            sb.AppendLine("P()= if(i<=SqrtOfN)");
            sb.AppendLine("	 {");
            sb.AppendLine("	 	 Q()");
            sb.AppendLine("	 }");
            sb.AppendLine("	 else");
            sb.AppendLine("	 {");
            sb.AppendLine("	 	Stop");
            sb.AppendLine("	 };");
            sb.AppendLine("	 ");
            sb.AppendLine("Q()=if(sievePrime[i]==0)");
            sb.AppendLine("	{");
            sb.AppendLine("		add{j=i;k=i+j;}->Sieve()");
            sb.AppendLine("	}");
            sb.AppendLine("	else");
            sb.AppendLine("	{");
            sb.AppendLine("		next{i=i+1;}->P()");
            sb.AppendLine("	};");
            sb.AppendLine("	");
            sb.AppendLine("Sieve()=if(k<=N)");
            sb.AppendLine("		{");
            sb.AppendLine("			sieve{sievePrime[k]=1;k=k+j;}->Sieve()");
            sb.AppendLine("		}");
            sb.AppendLine("		else");
            sb.AppendLine("		{");
            sb.AppendLine("			next{i=i+1;}->P()						");
            sb.AppendLine("		};");
            sb.AppendLine();

            string goal;
            goal = creatGoalForSieveofEratosthenes(N);
            sb.AppendLine(goal);
            sb.AppendLine("#assert P() reaches goal;");
            return sb.ToString();
        }

        private static string creatGoalForSieveofEratosthenes(int N)
        {
            string result = "#define goal (i==SqrtOfN+1";
            Boolean[] Prime = new Boolean[N + 1];
            int i;
            int j;
            for (i = 0; i < N + 1; i++)
            {
                Prime[i] = true;
            }
            for (i = 2; i <= Math.Sqrt(N); i++)
            {
                if (Prime[i])
                {
                    for (j = i * 2; j < N + 1; j = j + i)
                    {
                        Prime[j] = false;
                    }
                }
            }
            for (i = 2; i < N + 1; i++)
                if (Prime[i])
                {
                    result += "&&sievePrime[" + i.ToString() + "]==0";
                }
                else
                {
                    result += "&&sievePrime[" + i.ToString() + "]==1";
                }
            result += ");";
            return result;
        }

        public static string LoadFactorial(int N)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("#define N " + N + ";");
            sb.AppendLine("var factorial=1;");
            sb.AppendLine("var i=1;");
            sb.AppendLine("P()=if(i<=N) { mul{ factorial=factorial*i; i=i+1;}->P() } else { Stop };");
            sb.AppendLine("#define goal (i==N+1 && factorial==" + factorial(N) + ");");
            sb.AppendLine("#assert P() reaches goal;");
            return sb.ToString();
        }

        private static string factorial(int N)
        {
            int result = 1;
            for (int i = 1; i <= N; i++)
                result *= i;
            return result.ToString();
        }

        public static string LoadBiggestDivisor(int M, int N)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("var m = " + M.ToString() + ";");
            sb.AppendLine("var n = " + N.ToString() + ";");
            sb.AppendLine("var result = 0;");
            sb.AppendLine("P()=if(m>n) { sub{m=m-n;}->P() } else { Q() };");
            sb.AppendLine("Q()=if(m==n) { stop{result=m;}->Stop } else { sub{n=n-m;}->P() };");
            sb.AppendLine("#assert P() |= []<> stop;");
            sb.AppendLine("#define goal (result==" + BiggestDivisor(M, N).ToString() + ");");
            sb.AppendLine("#assert P() reaches goal;");
            return sb.ToString();
        }

        public static int BiggestDivisor(int M, int N)
        {
            if (M == 0)
                return N;
            if (N == 0)
                return M;
            if (M > N)
                return BiggestDivisor(M % N, N);
            else if (N > M)
                return BiggestDivisor(M, N % M);
            else
                return M;
        }


        public static string LoadPaceMaker()
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(
                "// *The amount of time we want to stimulate. Each cycles takes 50ms, so we stimulate for 400 cycles.");
            sb.AppendLine("// *We can change this value for longer or shorter for stimulate process.");
            sb.AppendLine("var env_simTime =2000;		        	");
            sb.AppendLine("");
            sb.AppendLine("var Steptime = 5;							// The step time of every loop.");
            sb.AppendLine("var currentTime = 0;			        	// Variable keeps track the time of the system");
            sb.AppendLine(
                "var mode = 2;				            	// The mode of the Pacemaker, 0 is OFF, 1 is VVI, 2 is DDD, 3 is DDDR");
            sb.AppendLine("var interval = 1000;			        	// The interval between pulses to achieve the expected rate");
            sb.AppendLine(
                "var threshold = 4;			        		// The value the accelerometer sensor output shall exceed before the pacemakers rate is affected by activity data");
            sb.AppendLine(
                "var env_outlines = [0,0,0];		    		// The output of the system includes [Pulse, Chamber, Time]");
            sb.AppendLine("var env_sense = 1;			           		// The sense of the input (NONE or PULSE)");
            sb.AppendLine("var env_chamber = 0;		            	// The chamber of the input (Atria or Ventricle)");
            sb.AppendLine(
                "var env_activityData = 4;		    		// The output of the accelerometer is defined as a value from 0 to 7 (0 is very low, 7 is very high and 4 is medium)");
            sb.AppendLine("var leadAtria_scheduleTime = 0;  			// The time we schedule to discharge a pulse at Atria");
            sb.AppendLine(
                "var leadAtria_schedulePulse = [0, 0];		// The time and kind of pulse we schedule to discharge at Atria");
            sb.AppendLine(
                "var leadVentricle_scheduleTime = 0;			// The time we schedule to discharge a pulse at Ventricle");
            sb.AppendLine(
                "var leadVentricle_schedulePulse = [0, 0];	// The time and kind of pulse we schedule to discharge at Ventricle");
            sb.AppendLine(
                "var heartCon_sensed = [0,0];	        	// The last sense for each chamber was kept track by an array with two elements [Chamber, Sense]");
            sb.AppendLine(
                "var rateCon_sensed = 4;		        		// The variable keeps track the last accelerometer value (1 is VERY LOW, 4 is MEDIUM, 7 is VERY HIGH)");
            sb.AppendLine("var heartCon_lastPulse = 0;					// The variable keeps track the last pulse");
            sb.AppendLine("var heartCon_lastVentricle = -1000;			// The time of last Ventrical	");
            sb.AppendLine("var heartCon_lastAtrial = -1000;			// The time of last Atrial");
            sb.AppendLine("var heartCon_lastPulseType = 0;				// The type of last pulse");
            sb.AppendLine("var heartCon_currentVentricle = -500;		// The time of latest Ventrical pulse");
            sb.AppendLine("var heartCon_currentAtrial = -150;			// The time of latest Artial pulse");
            sb.AppendLine(
                "var fixAV = 150;			            	// Programmable period from an atrial event to a ventricular pace");
            sb.AppendLine(
                "var intervalLRL = 1000;	        			// Lower Rate Limit: Number of pace pulses delivered per minute in the absence of sensed activity in an interval starting at a paced event");
            sb.AppendLine(
                "var intervalMSR = 500;		        		// Maximum Sensor Rate: Maximum pacing rate allowed as a result of accelerometer control");
            sb.AppendLine(
                "var VRP = 320;								// Ventrical Refractory Period: time interval following a ventricular event during which time ventricular senses shall not inhibit nor trigger pacing.");
            sb.AppendLine(
                "var ARP = 250;								// Atrial Refractory Period: this is the time interval following an atrial event during which time atrial events shall not inhibit or trigger pacing");
            sb.AppendLine(
                "var PVARP = 250;							// Post Ventricular Atrial Refractory Period: time interval following a ventricular event when an atrial cardiac event shall not 1. inhibit an atrial pace 2. trigger a ventricular pace, available to modes with atrial sensing");
            sb.AppendLine("var EPVARP = 0;								");
            sb.AppendLine("var bARP = 0;");
            sb.AppendLine("var bVRP = 0;");
            sb.AppendLine("var bPVARP = 0;");
            sb.AppendLine("var bPVC = 0;");
            sb.AppendLine("var bdetectATR = 0;");
            sb.AppendLine("var PVA = 0;");
            sb.AppendLine("var lastIsEPVA = 0;");
            sb.AppendLine("var fallback = 0;");
            sb.AppendLine("var ATRduration = 20;");
            sb.AppendLine("var intervalURL = 500;");
            sb.AppendLine("var ATRnum = 0;	");
            sb.AppendLine("var Atthreshold = 15;");
            sb.AppendLine("var changeStatus = 0;");
            sb.AppendLine("var FallbackFac = 1;");
            sb.AppendLine("var RecoFac = 1;");
            sb.AppendLine("var ReacFac = 1;");
            sb.AppendLine("");
            sb.AppendLine("// The system is the parallel process of environment and pace maker system");
            sb.AppendLine("System() = Valuation() || Pacemaker_system();");
            sb.AppendLine("");
            sb.AppendLine("// The environment to get the information from the sensor");
            sb.AppendLine("Valuation() = 	ifa (currentTime < env_simTime)");
            sb.AppendLine("					{");
            sb.AppendLine("						getSensorInfo -> env_CreateSignal();finishGetInfo -> Valuation()");
            sb.AppendLine("					}");
            sb.AppendLine("					else");
            sb.AppendLine("					{");
            sb.AppendLine("						endStimulation -> Skip");
            sb.AppendLine("					};");
            sb.AppendLine("");
            sb.AppendLine("// The CreateSignal process delivers stimuli to the different components");
            sb.AppendLine(
                "env_CreateSignal() = 	checktime1 -> env_Getsense(); env_Getchamber(); env_GetactivityData(); ");
            sb.AppendLine(
                "						getdata{heartCon_sensed[0] = env_chamber; heartCon_sensed[1] = env_sense; rateCon_sensed = env_activityData;} -> Skip;");
            sb.AppendLine("						");
            sb.AppendLine("");
            sb.AppendLine("// Get the mode of the pace maker");
            sb.AppendLine("env_Getmode()			=	getVVI{mode = 1;} -> Skip");
            sb.AppendLine("						[]	getDDD{mode = 2;} -> Skip");
            sb.AppendLine("						[]	getDDDR{mode = 3;} -> Skip;");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("// Get the sense of the event ");
            sb.AppendLine("env_Getsense() 			= 	ifa (currentTime % 500 == 0)");
            sb.AppendLine("							{");
            sb.AppendLine("								getSensePulse{env_sense = 1;} -> Skip");
            sb.AppendLine("							}");
            sb.AppendLine("							else");
            sb.AppendLine("							{");
            sb.AppendLine("								getSenseNone{env_sense = 0;} -> Skip");
            sb.AppendLine("							};");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("// Get the chamber of the event");
            sb.AppendLine("env_Getchamber()		=	getChamberAtria{env_chamber = 0;} -> Skip");
            sb.AppendLine("						[]	getChamberVentricle{env_chamber = 1;} -> Skip;");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("// Get the activity data of the event");
            sb.AppendLine("env_GetactivityData()	=	ifa (mode == 3)");
            sb.AppendLine("							{");
            sb.AppendLine("									getDataVeryLow{env_activityData = 1;} -> Skip");
            sb.AppendLine("								[]	getDataMedium{env_activityData = 4;} -> Skip");
            sb.AppendLine("								[]	getDataVeryHigh{env_activityData = 7;} -> Skip");
            sb.AppendLine("							}");
            sb.AppendLine("							else");
            sb.AppendLine("							{");
            sb.AppendLine("								getData{env_activityData = threshold;} -> Skip");
            sb.AppendLine("							};");
            sb.AppendLine("");
            sb.AppendLine("// Time is stepped each time");
            sb.AppendLine("IncreaseTime() = increaseTime{currentTime = currentTime + Steptime;} -> Skip;");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("// The main process, starting the system stimulation controlling the time");
            sb.AppendLine("// and system execution invoking the Step process on the system components");
            sb.AppendLine("Pacemaker_system() = getSensorInfo -> finishGetInfo ->");
            sb.AppendLine("								rateCon_Step(); ");
            sb.AppendLine("								heartCon_Step();");
            sb.AppendLine("								IncreaseTime();");
            sb.AppendLine("								Pacemaker_system();");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("// The right mode is chosen by the Pace process");
            sb.AppendLine("heartCon_Step() = [mode == 1]checkModeVVI -> heartCon_PaceVVI()");
            sb.AppendLine("				[][mode == 2]checkModeDDD -> heartCon_PaceDDD()");
            sb.AppendLine("				[][mode == 3]checkModeDDDR -> heartCon_PaceDDDR()");
            sb.AppendLine("				[][mode == 0]checkModeOFF -> Skip;");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine(
                "// Pace in the VVI mode follows from specification discarding all the sensed activity and pacing each time interval");
            sb.AppendLine("heartCon_PaceVVI()	= 	ifa (heartCon_sensed[0] == 1 && heartCon_sensed[1] == 1)");
            sb.AppendLine("						{");
            sb.AppendLine("							checkPaceVVI -> checkVRP();heartCon_checkSubPaceVVI()");
            sb.AppendLine("						}");
            sb.AppendLine("						else");
            sb.AppendLine("						{");
            sb.AppendLine("							ifa (interval + heartCon_currentVentricle <= currentTime)");
            sb.AppendLine("							{");
            sb.AppendLine("								checkPaceVVI2 {heartCon_lastPulse = env_outlines[2];heartCon_lastPulseType = env_outlines[0];");
            //sb.AppendLine("								//if (heartCon_lastPulseType == 1)");
            //sb.AppendLine("								//{");
            sb.AppendLine("									heartCon_lastVentricle = heartCon_currentVentricle;");
            //sb.AppendLine("								//}");
            //sb.AppendLine("								//else");
            //sb.AppendLine("								//{");
            //sb.AppendLine("								//	heartCon_lastAtrial = heartCon_currentAtrial;");
            //sb.AppendLine("								//}");
            sb.AppendLine(
                "								env_outlines[0] = 1;env_outlines[1] = 0;env_outlines[2] = currentTime; heartCon_currentVentricle = currentTime;								");
            sb.AppendLine("								}");
            sb.AppendLine("								-> Skip");
            sb.AppendLine("							}");
            sb.AppendLine("							else");
            sb.AppendLine("							{");
            sb.AppendLine("								checkPaceVVI3 -> Skip");
            sb.AppendLine("							}");
            sb.AppendLine("						};");
            sb.AppendLine("");
            sb.AppendLine("heartCon_checkSubPaceVVI()	=	ifa (bVRP == 1)");
            sb.AppendLine("								{");
            sb.AppendLine("									checkSubPaceVVI	{heartCon_lastPulse = env_outlines[2];heartCon_lastPulseType = env_outlines[0];");
            //sb.AppendLine("									//if (heartCon_lastPulseType == 1)");
            //sb.AppendLine("									//{");
            sb.AppendLine("										heartCon_lastVentricle = heartCon_currentVentricle;");
            //sb.AppendLine("									//}");
            //sb.AppendLine("									//else");
            //sb.AppendLine("									//{");
            //sb.AppendLine("									//	heartCon_lastAtrial = heartCon_currentAtrial;");
            //sb.AppendLine("									//}");
            sb.AppendLine(
                "									env_outlines[0] = 1;env_outlines[1] = 1;env_outlines[2] = currentTime;heartCon_currentVentricle = currentTime;");
            sb.AppendLine("									} -> Skip");
            sb.AppendLine("								}");
            sb.AppendLine("								else");
            sb.AppendLine("								{");
            sb.AppendLine("									checkSubPaceVVI2 -> Skip");
            sb.AppendLine("								};");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("// DDD mode provides triggered responses and discards refractory senses");
            sb.AppendLine("heartCon_PaceDDD() = 	ifa (heartCon_sensed[0] == 1 && heartCon_sensed[1] == 1)");
            sb.AppendLine("						{");
            sb.AppendLine("							checkPaceDDD -> checkVRP();heartCon_checkSubPaceDDD()");
            sb.AppendLine("						}");
            sb.AppendLine("						else");
            sb.AppendLine("						{");
            sb.AppendLine("							ifa (heartCon_sensed[0] == 0 && heartCon_sensed[1] == 1)");
            sb.AppendLine("							{");
            sb.AppendLine("								checkPaceDDD2 -> checkARP();checkPVARP();heartCon_checkSubPaceDDD2()");
            sb.AppendLine("							}");
            sb.AppendLine("							else");
            sb.AppendLine("							{");
            sb.AppendLine("								ifa (interval + heartCon_currentAtrial <= currentTime)");
            sb.AppendLine("								{");
            sb.AppendLine("									checkPaceDDD3 {heartCon_lastPulse = env_outlines[2];heartCon_lastPulseType = env_outlines[0];");
            //sb.AppendLine("									//if (heartCon_lastPulseType == 1)");
            //sb.AppendLine("									//{");
            //sb.AppendLine("									//	heartCon_lastVentricle = heartCon_currentVentricle;");
            //sb.AppendLine("									//}");
            //sb.AppendLine("									//else");
            //sb.AppendLine("									//{");
            sb.AppendLine("										heartCon_lastAtrial = heartCon_currentAtrial;");
            //sb.AppendLine("									//}");
            sb.AppendLine(
                "									env_outlines[0] = 0;env_outlines[1] = 0;env_outlines[2] = currentTime;heartCon_currentAtrial = currentTime;} -> Skip// -> checkFixAVDelay()");
            sb.AppendLine("						 		}");
            sb.AppendLine("						 		else");
            sb.AppendLine("						 		{");
            sb.AppendLine("						 			checkDDD3_2 -> checkFixAVDelay()");
            sb.AppendLine("						 		}");
            sb.AppendLine("						 	}");
            sb.AppendLine("						 };");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("heartCon_checkSubPaceDDD() =	ifa (bVRP == 1)");
            sb.AppendLine("								{");
            sb.AppendLine("									checkSubPaceDDD {heartCon_lastPulse = env_outlines[2];heartCon_lastPulseType = env_outlines[0];");
            //sb.AppendLine("									//if (heartCon_lastPulseType == 1)");
            //sb.AppendLine("									//{");
            sb.AppendLine("										heartCon_lastVentricle = heartCon_currentVentricle;");
            //sb.AppendLine("									//}");
            //sb.AppendLine("									//else");
            //sb.AppendLine("									//{");
            //sb.AppendLine("									//	heartCon_lastAtrial = heartCon_currentAtrial;");
            //sb.AppendLine("									//}");
            sb.AppendLine("									env_outlines[0] = 1;env_outlines[1] = 1;env_outlines[2] = currentTime;heartCon_currentVentricle = currentTime;} -> checkPVC()");
            sb.AppendLine("								}");
            sb.AppendLine("								else");
            sb.AppendLine("								{");
            sb.AppendLine("									checkSubPaceDDD_2 -> checkFixAVDelay()");
            sb.AppendLine("								};");
            sb.AppendLine("						");
            sb.AppendLine("heartCon_checkSubPaceDDD2() = 	ifa (bARP == 1 && bPVARP == 1)");
            sb.AppendLine("								{");
            sb.AppendLine(
                "									checkSubPaceDDD2 {heartCon_lastPulse = env_outlines[2];heartCon_lastPulseType = env_outlines[0];");
            //sb.AppendLine("									//if (heartCon_lastPulseType == 1)");
            //sb.AppendLine("									//{");
            //sb.AppendLine("									//	heartCon_lastVentricle = heartCon_currentVentricle;");
            //sb.AppendLine("									//}");
            //sb.AppendLine("									//else");
            //sb.AppendLine("									//{");
            sb.AppendLine("										heartCon_lastAtrial = heartCon_currentAtrial;");
            //sb.AppendLine("									//}");
            sb.AppendLine(
                "									env_outlines[0] = 0;env_outlines[1] = 1;env_outlines[2] = currentTime;heartCon_currentAtrial = currentTime;} -> checkATR()");
            sb.AppendLine("							 	}");
            sb.AppendLine("							 	else");
            sb.AppendLine("							 	{");
            sb.AppendLine("							 		checkSubPaceDDD2_2 -> checkFixAVDelay()");
            sb.AppendLine("							 	};");
            sb.AppendLine("						");
            sb.AppendLine("");
            sb.AppendLine(
                "// Pace in the DDDR mode discards all the sensed activity and stimulates both atria and ventricle with a fixed delay each time interval");
            sb.AppendLine("heartCon_PaceDDDR() = 	ifa (heartCon_sensed[0] == 1 && heartCon_sensed[1] == 1)");
            sb.AppendLine("						{");
            sb.AppendLine("							checkPaceDDDR -> checkVRP();heartCon_checkSubPaceDDDR()");
            sb.AppendLine("						}");
            sb.AppendLine("						else");
            sb.AppendLine("						{");
            sb.AppendLine("							ifa (heartCon_sensed[0] == 0 && heartCon_sensed[1] == 1)");
            sb.AppendLine("							{");
            sb.AppendLine("								checkPaceDDDR2 -> checkARP();checkPVARP();heartCon_checkSubPaceDDDR2()");
            sb.AppendLine("							}");
            sb.AppendLine("							else");
            sb.AppendLine("							{");
            sb.AppendLine("								ifa (interval + heartCon_currentAtrial <= currentTime)");
            sb.AppendLine("								{");
            sb.AppendLine(
                "									checkPaceDDDR3 {heartCon_lastPulse = env_outlines[2];heartCon_lastPulseType = env_outlines[0];");
            //sb.AppendLine("									//if (heartCon_lastPulseType == 1)");
            //sb.AppendLine("									//{");
            //sb.AppendLine("									//	heartCon_lastVentricle = heartCon_currentVentricle;");
            //sb.AppendLine("									//}");
            //sb.AppendLine("									//else");
            //sb.AppendLine("									//{");
            sb.AppendLine("										heartCon_lastAtrial = heartCon_currentAtrial;");
            //sb.AppendLine("									//}");
            sb.AppendLine(
                "									env_outlines[0] = 0;env_outlines[1] = 0;env_outlines[2] = currentTime;heartCon_currentAtrial = currentTime;} -> Skip// -> checkFixAVDelay()");
            sb.AppendLine("						 		}");
            sb.AppendLine("						 		else");
            sb.AppendLine("						 		{");
            sb.AppendLine("						 			checkDDDR3_2 -> checkFixAVDelay()");
            sb.AppendLine("						 		}");
            sb.AppendLine("						 	}");
            sb.AppendLine("						 };");
            sb.AppendLine("");
            sb.AppendLine("heartCon_checkSubPaceDDDR() = 	ifa (bVRP == 1)");
            sb.AppendLine("								{");
            sb.AppendLine(
                "									checkSubPaceDDDR {heartCon_lastPulse = env_outlines[2];heartCon_lastPulseType = env_outlines[0];");
            //sb.AppendLine("									//if (heartCon_lastPulseType == 1)");
            //sb.AppendLine("									//{");
            sb.AppendLine("										heartCon_lastVentricle = heartCon_currentVentricle;");
            //sb.AppendLine("									//}");
            //sb.AppendLine("									//else");
            //sb.AppendLine("									//{");
            //sb.AppendLine("									//	heartCon_lastAtrial = heartCon_currentAtrial;");
            //sb.AppendLine("									//}");
            sb.AppendLine(
                "									env_outlines[0] = 1;env_outlines[1] = 1;env_outlines[2] = currentTime;heartCon_currentVentricle = currentTime;} -> checkPVC()");
            sb.AppendLine("							 	}");
            sb.AppendLine("							 	else");
            sb.AppendLine("							 	{");
            sb.AppendLine("							 		checkSubPaceDDDR_2 -> checkFixAVDelay()");
            sb.AppendLine("							 	};");
            sb.AppendLine("						");
            sb.AppendLine("heartCon_checkSubPaceDDDR2() = 	ifa (bARP == 1 && bPVARP == 1)");
            sb.AppendLine("								{");
            sb.AppendLine("									checkSubPaceDDDR2 {heartCon_lastPulse = env_outlines[2];heartCon_lastPulseType = env_outlines[0];");
            //sb.AppendLine("									//if (heartCon_lastPulseType == 1)");
            //sb.AppendLine("									//{");
            //sb.AppendLine("									//	heartCon_lastVentricle = heartCon_currentVentricle;");
            //sb.AppendLine("									//}");
            //sb.AppendLine("									//else");
            //sb.AppendLine("									//{");
            sb.AppendLine("										heartCon_lastAtrial = heartCon_currentAtrial;");
            //sb.AppendLine("									//}");
            sb.AppendLine(
                "									env_outlines[0] = 0;env_outlines[1] = 1;env_outlines[2] = currentTime;heartCon_currentAtrial = currentTime;} -> checkATR()");
            sb.AppendLine("							 	}");
            sb.AppendLine("							 	else");
            sb.AppendLine("							 	{");
            sb.AppendLine("							 		checkSubPaceDDDR2_2 -> checkFixAVDelay()");
            sb.AppendLine("							 	};");
            sb.AppendLine("");
            sb.AppendLine("checkARP()			=	ifa (currentTime - heartCon_currentAtrial >= ARP)");
            sb.AppendLine("						{");
            sb.AppendLine("							checkARP1{bARP = 1;} -> Skip");
            sb.AppendLine("						}");
            sb.AppendLine("						else");
            sb.AppendLine("						{");
            sb.AppendLine("							checkARP2{bARP = 0;} -> Skip");
            sb.AppendLine("						};");
            sb.AppendLine("					");
            sb.AppendLine("");
            sb.AppendLine("checkVRP()			=	ifa (currentTime - heartCon_currentVentricle >= VRP)");
            sb.AppendLine("						{");
            sb.AppendLine("							checkVRP1{bVRP = 1;} -> Skip");
            sb.AppendLine("						}");
            sb.AppendLine("						else");
            sb.AppendLine("						{");
            sb.AppendLine("							checkVRP2{bVRP = 0;} -> Skip");
            sb.AppendLine("						};");
            sb.AppendLine("					");
            sb.AppendLine("");
            sb.AppendLine("checkPVARP()		=	ifa (currentTime - heartCon_currentVentricle >= PVA)");
            sb.AppendLine("						{");
            sb.AppendLine("							checkPVARP1{bPVARP = 1;} -> Skip");
            sb.AppendLine("						}");
            sb.AppendLine("						else");
            sb.AppendLine("						{");
            sb.AppendLine("							checkPVARP2{bPVARP = 0;} -> Skip");
            sb.AppendLine("						};");
            sb.AppendLine("");
            sb.AppendLine("checkPVC()			=	ifa (heartCon_lastPulseType == 1)");
            sb.AppendLine("						{");
            sb.AppendLine("							checkPVC1{bPVC = 1;}-> checksubPVC()");
            sb.AppendLine("						}");
            sb.AppendLine("						else");
            sb.AppendLine("						{");
            sb.AppendLine("							checkPVC2{bPVC = 0; lastIsEPVA = 0;} -> checksubPVC()");
            sb.AppendLine("						};");
            sb.AppendLine("					");
            sb.AppendLine("checksubPVC()		=	ifa (bPVC == 1 && lastIsEPVA == 0)");
            sb.AppendLine("						{");
            sb.AppendLine("							checksubPVC1{PVA = EPVARP;lastIsEPVA = 1; bPVC = 1;} -> Skip");
            sb.AppendLine("						}");
            sb.AppendLine("						else");
            sb.AppendLine("						{");
            sb.AppendLine("							checksubPVC2{PVA = PVARP;} -> Skip");
            sb.AppendLine("						};");
            sb.AppendLine("					");
            sb.AppendLine("checkATR()			=	checkATR1 -> detectATR();checksubATR();");
            sb.AppendLine("");
            sb.AppendLine("checksubATR()		=	ifa (bdetectATR == 1)");
            sb.AppendLine("						{");
            sb.AppendLine("							checksubATR1{fallback = 1;} -> Skip");
            sb.AppendLine("						}");
            sb.AppendLine("						else");
            sb.AppendLine("						{");
            sb.AppendLine("							ifa (bdetectATR == 0)");
            sb.AppendLine("							{");
            sb.AppendLine("								checksubATR2{fallback = 0;} -> Skip");
            sb.AppendLine("							}");
            sb.AppendLine("							else");
            sb.AppendLine("							{");
            sb.AppendLine("								checksubATR3 -> Skip");
            sb.AppendLine("							}");
            sb.AppendLine("						};");
            sb.AppendLine("					");
            sb.AppendLine("detectATR()			=	checkdetectATR{ATRduration = ATRduration - 1;} -> subdetectATR();");
            sb.AppendLine("");
            sb.AppendLine("subdetectATR()		=	ifa (ATRduration > 0)");
            sb.AppendLine("						{");
            sb.AppendLine("							ifa (heartCon_currentAtrial - heartCon_lastAtrial <= intervalURL)");
            sb.AppendLine("							{	");
            sb.AppendLine("								checksubdetectATR1 {ATRnum = ATRnum + 1;bdetectATR = -1;} -> Skip");
            sb.AppendLine("							}");
            sb.AppendLine("						}");
            sb.AppendLine("						else");
            sb.AppendLine("						{");
            sb.AppendLine("							ifa (ATRnum >  Atthreshold)");
            sb.AppendLine("							{");
            sb.AppendLine("								checksubdetectATR2 {ATRduration = 20;ATRnum = 0;bdetectATR = 1;} -> Skip");
            sb.AppendLine("							}");
            sb.AppendLine("							else");
            sb.AppendLine("							{");
            sb.AppendLine("								checksubdetectATR3 {ATRduration = 20;ATRnum = 0;bdetectATR = 0;} -> Skip");
            sb.AppendLine("							}");
            sb.AppendLine("						};");
            sb.AppendLine("					");
            sb.AppendLine("checkFixAVDelay()	=	ifa (heartCon_currentAtrial + fixAV == currentTime && currentTime - heartCon_currentVentricle >= intervalURL)// && env_outlines[0] == 0)");
            sb.AppendLine("						{");
            sb.AppendLine(
                "							checkfixAVdelay1 {heartCon_lastPulse = env_outlines[2];heartCon_lastPulseType = env_outlines[0];");
            //sb.AppendLine("							//if (heartCon_lastPulseType == 1)");
            //sb.AppendLine("							//{");
            sb.AppendLine("								heartCon_lastVentricle = heartCon_currentVentricle;");
            //sb.AppendLine("							//}");
            //sb.AppendLine("							//else");
            //sb.AppendLine("							//{");
            //sb.AppendLine("							//	heartCon_lastAtrial = heartCon_currentAtrial;");
            //sb.AppendLine("							//}");
            sb.AppendLine(
                "							env_outlines[0] = 1;env_outlines[1] = 0;env_outlines[2] = currentTime;heartCon_currentVentricle = currentTime;} -> Skip");
            sb.AppendLine("						}");
            sb.AppendLine("						else");
            sb.AppendLine("						{");
            sb.AppendLine("							checkfixAVdelay2 -> Skip");
            sb.AppendLine("						};");
            sb.AppendLine("					");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine(
                "// Each time step the controlRate process will be invoked if there was some input from the accelerometer");
            sb.AppendLine("rateCon_Step() = 	ifa (heartCon_sensed[1] != 0 || changeStatus !=0)");
            sb.AppendLine("					{");
            sb.AppendLine("						checkRateConSensed1 -> rateCon_ControlRate()");
            sb.AppendLine("					}");
            sb.AppendLine("					else");
            sb.AppendLine("					{");
            sb.AppendLine("						checkRateConSensed2 -> Skip");
            sb.AppendLine("					};");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("// The control of the rate is done regarding a threshold");
            sb.AppendLine("rateCon_ControlRate() 	= 	ifa (fallback == 1)");
            sb.AppendLine("							{");
            sb.AppendLine("								checkControlRate1 -> decreaseRate()");
            sb.AppendLine("							}");
            sb.AppendLine("							else");
            sb.AppendLine("							{");
            sb.AppendLine("								ifa (rateCon_sensed > threshold || changeStatus == 1)");
            sb.AppendLine("								{");
            sb.AppendLine("									checkSubControlRate1 -> increaseRate();resetSensed()");
            sb.AppendLine("								}");
            sb.AppendLine("								else");
            sb.AppendLine("								{");
            sb.AppendLine("									ifa (rateCon_sensed < threshold || changeStatus == 2)");
            sb.AppendLine("									{");
            sb.AppendLine("										checkSubControlRate2 -> decreaseRate();resetSensed()");
            sb.AppendLine("									}");
            sb.AppendLine("									else");
            sb.AppendLine("									{");
            sb.AppendLine("										checkSubControlRate3 -> Skip");
            sb.AppendLine("									}");
            sb.AppendLine("								}");
            sb.AppendLine("							};");
            sb.AppendLine("						");
            sb.AppendLine("resetSensed()			= 	resetSense{rateCon_sensed = 0;} -> Skip; ");
            sb.AppendLine("						");
            sb.AppendLine("increaseRate()			= 	ifa (mode == 2 || mode == 3)");
            sb.AppendLine("						 	{");
            sb.AppendLine("						 		checkincreaseRatemode1 -> increaseDDDmodeRate()");
            sb.AppendLine("						 	}");
            sb.AppendLine("						 	else");
            sb.AppendLine("						 	{");
            sb.AppendLine("						 		checkincreaseRatemode2 -> Skip");
            sb.AppendLine("						 	};");
            sb.AppendLine("");
            sb.AppendLine("increaseDDDmodeRate()	= 	ifa (interval > intervalMSR)");
            sb.AppendLine("							{");
            sb.AppendLine("								ifa (mode == 3)");
            sb.AppendLine("								{");
            sb.AppendLine(
                "									checkincreaseDDDmodeRate2 {interval = interval - ReacFac; changeStatus = 1;} -> Skip");
            sb.AppendLine("								}");
            sb.AppendLine("								else");
            sb.AppendLine("								{");
            sb.AppendLine("									checkincreaseDDDmodeRate3 {interval = intervalMSR; changeStatus = 0;} -> Skip");
            sb.AppendLine("								}								");
            sb.AppendLine("							}");
            sb.AppendLine("							else");
            sb.AppendLine("							{");
            sb.AppendLine("								checkincreaseDDDmodeRate4 {changeStatus = 0;} -> Skip");
            sb.AppendLine("							};");
            sb.AppendLine("							");
            sb.AppendLine("");
            sb.AppendLine("decreaseRate()			= 	ifa (mode == 2 || mode == 3)");
            sb.AppendLine("							{");
            sb.AppendLine("								checkdecreaseRatemode1 -> decreaseDDDmodeRate()");
            sb.AppendLine("							}");
            sb.AppendLine("							else");
            sb.AppendLine("							{");
            sb.AppendLine("								checkdecreaseRatemode2 -> Skip");
            sb.AppendLine("							};");
            sb.AppendLine("						");
            sb.AppendLine("decreaseDDDmodeRate()	=	ifa (interval < intervalLRL)");
            sb.AppendLine("							{");
            sb.AppendLine("								ifa (fallback == 1)");
            sb.AppendLine("								{");
            sb.AppendLine(
                "									checkdecreaseDDDmodeRate1 {interval = interval + FallbackFac; changeStatus = 2;} -> Skip");
            sb.AppendLine("								}");
            sb.AppendLine("								else");
            sb.AppendLine("								{");
            sb.AppendLine("									ifa (mode == 3)");
            sb.AppendLine("									{");
            sb.AppendLine(
                "										checkdecreaseDDDmodeRate2 {interval = interval + RecoFac; changeStatus = 2;} -> Skip");
            sb.AppendLine("									}");
            sb.AppendLine("									else");
            sb.AppendLine("									{");
            sb.AppendLine("										checkdecreaseDDDmodeRate3 {interval = intervalLRL;changeStatus = 0;} -> Skip");
            sb.AppendLine("									}");
            sb.AppendLine("								}");
            sb.AppendLine("							}");
            sb.AppendLine("							else");
            sb.AppendLine("							{");
            sb.AppendLine("								checkdecreaseDDDmodeRate3 {changeStatus = 0;} -> Skip");
            sb.AppendLine("							};");
            sb.AppendLine("							");
            sb.AppendLine(" ");
            sb.AppendLine("");
            sb.AppendLine("//////////////////////// Properties //////////////////////////////");
            sb.AppendLine("");
            sb.AppendLine(
                "#define m (mode != 1 && heartCon_currentVentricle > heartCon_currentAtrial && heartCon_lastVentricle < heartCon_currentAtrial);");
            sb.AppendLine(
                "#define e0 (heartCon_currentVentricle  - heartCon_currentAtrial >= fixAV - 8 && heartCon_currentVentricle - heartCon_currentAtrial <= fixAV + 8);");
            sb.AppendLine("#define e1 heartCon_currentAtrial - heartCon_lastAtrial <= 1008;");
            sb.AppendLine("#define e2 heartCon_currentVentricle - heartCon_lastVentricle <= 1008;");
            sb.AppendLine("#define e3 heartCon_currentVentricle - heartCon_lastVentricle >= VRP - 8;");
            sb.AppendLine("#define e4 heartCon_currentAtrial - heartCon_lastAtrial >= ARP - 8;");
            sb.AppendLine("");
            sb.AppendLine(
                "// Fixed AV delay requirement: after an atrial event there must be a ventricular pace after 150ms (+/- 8ms)");
            sb.AppendLine("#assert System() |= [](m -> e0);");
            sb.AppendLine("");
            sb.AppendLine("// LRL requirement: The maximum delay allowed between pulses is 1000 ms (+/- 8ms)");
            sb.AppendLine("#assert System() |= []e1;");
            sb.AppendLine("");
            sb.AppendLine("#assert System() |= []e2;");
            sb.AppendLine("");
            sb.AppendLine("// A mode Tansitive pacemaker must always confirm that a pulse is issued every time a heartbeat is detected while it is in Triggered pacing mode");
            sb.AppendLine("//#assert System() |= [](P1 -> <> P2);");
            sb.AppendLine("");
            sb.AppendLine("// VRP requirement: the minimum delay allowed between two consecutive ventrical paces.");
            sb.AppendLine("#assert System() |= []e3;");
            sb.AppendLine("");
            sb.AppendLine("// ARP requirement: the minimum delay allowed between two consecutive atrial paces.");
            sb.AppendLine("#assert System() |= []e4;");

            return sb.ToString();
        }

        public static string LoadTimedPaceMaker()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/* Programmable parameters */");
            sb.AppendLine("#define LRI 1000; //Lower Rate Interval");
            sb.AppendLine("#define URI 500;//Upper Rate Interval");
            sb.AppendLine("#define MSI 500;");
            sb.AppendLine("#define AVDelay 150; //Atria-ventricle delay");
            sb.AppendLine("#define ARP 400;");
            sb.AppendLine("#define VRP 350;");
            sb.AppendLine("#define PVARP 250;");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("/* System Variables */");
            sb.AppendLine("var SA = 1;//Allow Sensing Atria");
            sb.AppendLine("var SV = 1;//Allow Sensing Ventricle");
            sb.AppendLine("");
            sb.AppendLine("var interval = 500;");
            sb.AppendLine("var Hinterval = 1000;");
            sb.AppendLine("");
            sb.AppendLine("/* Heart Behaviors */");
            sb.AppendLine("#define HAOFFSET 50;");
            sb.AppendLine("#define HVOFFSET 50;");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("/* Model a human heart */");
            sb.AppendLine("Heart() =");
            sb.AppendLine("       //normal case");
            sb.AppendLine("       (pulseA -> Wait[AVDelay])within[0];(pulseV -> Wait[Hinterval-AVDelay])within[0];Heart()");
            sb.AppendLine("       <> //pulseAmissing");
            sb.AppendLine("       (pulseAmissing -> Wait[AVDelay])within[0];(pulseV -> Wait[Hinterval-AVDelay])within[0];Heart()");
            sb.AppendLine("       <> //pulseVmissing");
            sb.AppendLine("       (pulseA -> Wait[AVDelay])within[0];(pulseVmissing -> Wait[Hinterval-AVDelay])within[0];Heart()");
            sb.AppendLine("       <> //dead heart");
            sb.AppendLine("       (pulseAmissing -> Wait[AVDelay])within[0];(pulseVmissing -> Wait[Hinterval-AVDelay])within[0];Heart()");
            sb.AppendLine("       <> //slow pulseA");
            sb.AppendLine("       (pulseA -> Wait[AVDelay + HVOFFSET])within[0];(pulseV -> Wait[Hinterval-AVDelay])within[0];Heart()");
            sb.AppendLine("       <> //slow pulseV");
            sb.AppendLine("       (pulseA -> Wait[AVDelay])within[0];(pulseV -> Wait[Hinterval-AVDelay+ HAOFFSET])within[0];Heart();");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("/* Sensor in Atria */");
            sb.AppendLine("AtriaSensor = if(SA == 1) {atomic{pulseA -> senseA -> Skip}; AtriaSensor} else {AtriaSensor}");
            sb.AppendLine("             []");
            sb.AppendLine("             if(SA == 0) {pulseA -> AtriaSensor} else {AtriaSensor};");
            sb.AppendLine("");
            sb.AppendLine("/* Sensor in Ventricle */");
            sb.AppendLine("VentricleSensor =");
            sb.AppendLine("                       if(SV == 1) {atomic{pulseV -> senseV -> Skip}; VentricleSensor} else {VentricleSensor}");
            sb.AppendLine("            []");
            sb.AppendLine("            if(SV == 0) {pulseV -> VentricleSensor}  else {VentricleSensor};");
            sb.AppendLine("");
            sb.AppendLine("/* Rate Controller */");
            sb.AppendLine(" /* Define values of activity data, i.e., the human's active level.");
            sb.AppendLine(" * When activity data is low, heart beat is slow, and vice versa.");
            sb.AppendLine(" */");
            sb.AppendLine(" #define VLOW 0;");
            sb.AppendLine(" #define LOW 1;");
            sb.AppendLine(" #define MEDLOW 2;");
            sb.AppendLine(" #define MED 3;");
            sb.AppendLine(" #define MEDHIGH 4;");
            sb.AppendLine(" #define HIGH 5;");
            sb.AppendLine(" #define VHIGH 6;");
            sb.AppendLine("");
            sb.AppendLine(" #define RateSmoothing 20;");
            sb.AppendLine(" #define ReactionTime 30000;");
            sb.AppendLine(" #define RecoveryTime 300000;");
            sb.AppendLine("");
            sb.AppendLine("var actData = VHIGH;");
            sb.AppendLine("var sensedAct = VHIGH;");
            sb.AppendLine("var newInterval = 500;");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("/* Activity Sensor and Ratecontroller */");
            sb.AppendLine("ActSensorAndRateControll =");
            sb.AppendLine("                (senseNone{sensedAct = actData} -> RateController");
            sb.AppendLine("               <>senseVLow{sensedAct = VLOW} -> RateController");
            sb.AppendLine("               <>senseLOW{sensedAct = LOW} -> RateController");
            sb.AppendLine("               <>senseMEDLOW{sensedAct = MEDLOW} -> RateController");
            sb.AppendLine("               <>senseMED{sensedAct = MED} -> RateController");
            sb.AppendLine("               <>senseMEDHIGH{sensedAct = MEDHIGH} -> RateController");
            sb.AppendLine("               <>senseHIGH{sensedAct = HIGH} -> RateController");
            sb.AppendLine("               <>senseVHIGH{sensedAct = VHIGH} -> RateController)within[0];");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("RateController = (rateControlling{");
            sb.AppendLine("                                       if(sensedAct != actData)");
            sb.AppendLine("                                       {");
            sb.AppendLine("                                               actData = sensedAct;");
            sb.AppendLine("                                               newInterval = URI + (LRI - URI)*(VHIGH-sensedAct)/VHIGH;");
            sb.AppendLine("                                       }");
            sb.AppendLine("                                       if(interval < newInterval){");
            sb.AppendLine("                                               interval = interval + interval *  (LRI - MSI)/RecoveryTime;");
            sb.AppendLine("                                               if (interval > newInterval) interval = newInterval;");
            sb.AppendLine("                                       }");
            sb.AppendLine("                                       else if(interval > newInterval){");
            sb.AppendLine("                                               interval = interval - interval *  (LRI - MSI)/ReactionTime;");
            sb.AppendLine("                                               if (interval < newInterval) interval = newInterval;");
            sb.AppendLine("                                       }");
            sb.AppendLine("                               } -> Skip)within[0];");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("/* AAT mode */");
            sb.AppendLine("Sys_AAT = (Heart || AtriaSensor || AATpace)\\{senseA, pulseA, pulseV, pulseAmissing,pulseVmissing,enableSA};");
            sb.AppendLine("");
            sb.AppendLine("// Whenever an atrial event(either sensed or paced) happens, atrial senses in the following ARP's time is ignored.");
            sb.AppendLine("AATpace =  (atomic{senseA -> paceA{SA = 0} -> Skip} timeout[LRI] ((paceA{SA = 0} -> Skip) within[0] ); Wait[URI]; (enableSA{SA = 1} -> AATpace1) within[0]);");
            sb.AppendLine("");
            sb.AppendLine("AATpace1 = (atomic{senseA -> paceA{SA = 0} -> Skip} timeout[LRI-URI] ((paceA{SA = 0} -> Skip) within[0] ) ; Wait[URI]; (enableSA{SA = 1} -> AATpace1) within[0]);");
            sb.AppendLine("");
            sb.AppendLine("SPEC_AAT = paceA -> Loop_AAT;");
            sb.AppendLine("Loop_AAT = (paceA -> Loop_AAT) within[URI, LRI];");
            sb.AppendLine("");
            sb.AppendLine("#assert Sys_AAT refines <T> SPEC_AAT;");
            sb.AppendLine("#assert Sys_AAT deadlockfree;");
            sb.AppendLine("");
            sb.AppendLine("/* VVT mode */");
            sb.AppendLine("Sys_VVT = (Heart || VentricleSensor || VVTpace)\\{senseV, pulseA, pulseV, pulseAmissing,pulseVmissing,enableSV};");
            sb.AppendLine("");
            sb.AppendLine("// Whenever an atrial event(either sensed or paced) happens, atrial senses in the following ARP's time is ignored.");
            sb.AppendLine("VVTpace =  (atomic{senseV -> paceV{SV = 0} -> Skip} timeout[LRI] ((paceV{SV = 0} -> Skip) within[0] ); Wait[URI]; (enableSV{SV = 1} -> VVTpace1) within[0]);");
            sb.AppendLine("");
            sb.AppendLine("VVTpace1 = (atomic{senseV -> paceV{SV = 0} -> Skip} timeout[LRI-URI] ((paceV{SV = 0} -> Skip) within[0] ) ; Wait[URI]; (enableSV{SV = 1} -> VVTpace1) within[0]);");
            sb.AppendLine("");
            sb.AppendLine("SPEC_VVT = paceV -> Loop_VVT;");
            sb.AppendLine("Loop_VVT = (paceV -> Loop_VVT) within[URI, LRI];");
            sb.AppendLine("");
            sb.AppendLine("#assert Sys_VVT refines <T> SPEC_VVT;");
            sb.AppendLine("#assert Sys_VVT deadlockfree;");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("/* AOO mode*/");
            sb.AppendLine("Sys_AOO = (Heart || AOOpace)\\{senseA, pulseA, pulseV, pulseAmissing,pulseVmissing};");
            sb.AppendLine("");
            sb.AppendLine("//Pace the atrium without sensing");
            sb.AppendLine("AOOpace = (paceA -> Wait[interval])within[0];AOOpace;");
            sb.AppendLine("");
            sb.AppendLine("SPEC_AOO = paceA -> Loop_AOO;");
            sb.AppendLine("Loop_AOO = (paceA -> Loop_AOO) within[interval];");
            sb.AppendLine("");
            sb.AppendLine("#assert Sys_AOO refines <T> SPEC_AOO;");
            sb.AppendLine("#assert Sys_AOO deadlockfree;");
            sb.AppendLine("");
            sb.AppendLine("/* AOOR mode */");
            sb.AppendLine("Sys_AOOR = (Heart || AOORpace)\\{senseA, pulseA, pulseV, pulseAmissing,pulseVmissing,");
            sb.AppendLine("                               senseNone, senseVLow, senseLOW, senseMEDLOW, senseMED, senseMEDHIGH,");
            sb.AppendLine("                               senseHIGH, senseVHIGH, rateControlling};");
            sb.AppendLine("");
            sb.AppendLine("AOORpace = ActSensorAndRateControll;AOORpace1;AOORpace;");
            sb.AppendLine("AOORpace1 = (paceA -> Wait[interval])within[0];");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("SPEC_AOOR = paceA -> LoopAOOR;");
            sb.AppendLine("LoopAOOR = (paceA -> LoopAOOR)within[interval];");
            sb.AppendLine("");
            sb.AppendLine("#assert Sys_AOOR deadlockfree;");
            sb.AppendLine("#assert Sys_AOOR refines <T> SPEC_AOOR;");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("/* AAI mode */");
            sb.AppendLine("Sys_AAI = (Heart || AtriaSensor || AAIpace)\\{pulseA, pulseV, pulseAmissing,pulseVmissing,enableSA, disableSA, senseA};");
            sb.AppendLine("");
            sb.AppendLine("//A sense in the atrium will inhibit a pending pace event");
            sb.AppendLine("AAIpace =  ((atomic{senseA -> paceA{SA = 0} -> Skip}) timeout[LRI] ((paceA{SA = 0} -> Skip) within[0]); Wait[URI]; (enableSA{SA = 1} -> AAIpace1) within[0]);");
            sb.AppendLine("");
            sb.AppendLine("AAIpace1 = ((atomic{senseA -> paceA{SA = 0} -> Skip}) timeout[LRI - URI] ((paceA{SA = 0} -> Skip) within[0]); Wait[URI]; (enableSA{SA = 1} -> AAIpace1) within[0]);");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("SPEC_AAI = paceA -> Loop_AAI;");
            sb.AppendLine("Loop_AAI = (paceA -> Loop_AAI) within[URI, LRI];");
            sb.AppendLine("");
            sb.AppendLine("#assert Sys_AAI refines <T> SPEC_AAI;");
            sb.AppendLine("#assert Sys_AAI deadlockfree;");
            sb.AppendLine("");
            sb.AppendLine("/* AAIR mode */");
            sb.AppendLine("Sys_AAIR = (Heart || AtriaSensor || AAIRpace)\\{senseA, pulseA, pulseV, pulseAmissing,pulseVmissing,enableSA,");
            sb.AppendLine("                               senseNone, senseVLow, senseLOW, senseMEDLOW, senseMED, senseMEDHIGH,");
            sb.AppendLine("                               senseHIGH, senseVHIGH, rateControlling};");
            sb.AppendLine("");
            sb.AppendLine("AAIRpace = ActSensorAndRateControll;AAIRpace1;AAIRpace2;");
            sb.AppendLine("AAIRpace2 = ActSensorAndRateControll;AAIRpace3;AAIRpace2;");
            sb.AppendLine("");
            sb.AppendLine("AAIRpace1 =  ((atomic{senseA -> paceA{SA = 0} -> Skip}) timeout[interval] ((paceA{SA = 0} -> Skip) within[0]); Wait[ARP]; (enableSA{SA = 1} -> Skip) within[0]);");
            sb.AppendLine("");
            sb.AppendLine("AAIRpace3 = ((atomic{senseA -> paceA{SA = 0} -> Skip}) timeout[interval - ARP] ((paceA{SA = 0} -> Skip) within[0]); Wait[ARP]; (enableSA{SA = 1} -> Skip) within[0]);");
            sb.AppendLine("");
            sb.AppendLine("SPEC_AAIR = paceA -> Loop_AAIR;");
            sb.AppendLine("Loop_AAIR = (paceA -> Loop_AAIR) within[interval];");
            sb.AppendLine("");
            sb.AppendLine("#assert Sys_AAIR refines <T> SPEC_AAIR;");
            sb.AppendLine("#assert Sys_AAIR deadlockfree;");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("/* VOO mode */");
            sb.AppendLine("Sys_VOO = (Heart || VentricleSensor || VOOpace)\\{senseV, pulseA, pulseV, pulseAmissing,pulseVmissing,enableSA};");
            sb.AppendLine("");
            sb.AppendLine("//Pace the ventricles without sensing");
            sb.AppendLine("VOOpace = (paceV -> Wait[interval])within[0];VOOpace;");
            sb.AppendLine("");
            sb.AppendLine("SPEC_VOO = paceV -> Loop_VOO;");
            sb.AppendLine("Loop_VOO = (paceV -> Loop_VOO) within[interval];");
            sb.AppendLine("");
            sb.AppendLine("#assert Sys_VOO refines <T> SPEC_VOO;");
            sb.AppendLine("#assert Sys_VOO deadlockfree;");
            sb.AppendLine("");
            sb.AppendLine("/* VOOR mode */");
            sb.AppendLine("Sys_VOOR = (Heart || VOORpace)\\{senseA, pulseA, pulseV, pulseAmissing,pulseVmissing,");
            sb.AppendLine("                               senseNone, senseVLow, senseLOW, senseMEDLOW, senseMED, senseMEDHIGH,");
            sb.AppendLine("                               senseHIGH, senseVHIGH, rateControlling};");
            sb.AppendLine("");
            sb.AppendLine("VOORpace = ActSensorAndRateControll;(paceV -> Wait[interval])within[0];VOORpace;");
            sb.AppendLine("");
            sb.AppendLine("SPEC_VOOR = paceV -> LoopVOOR;");
            sb.AppendLine("LoopVOOR = (paceV -> LoopVOOR)within[interval];");
            sb.AppendLine("");
            sb.AppendLine("#assert Sys_VOOR deadlockfree;");
            sb.AppendLine("#assert Sys_VOOR refines <T> SPEC_VOOR;");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("/* VVI mode */");
            sb.AppendLine("Sys_VVI = (Heart || VentricleSensor || VVIpace)\\{pulseA, pulseV, pulseAmissing,pulseVmissing,enableSV, disableSV, senseV};");
            sb.AppendLine("");
            sb.AppendLine("//A sense in the ventricle will inhibit a pending pace event");
            sb.AppendLine("VVIpace =  ((atomic{senseV -> paceV{SV = 0} -> Skip}) timeout[LRI] ((paceV{SV = 0} -> Skip) within[0]); Wait[URI]; (enableSV{SV = 1} -> VVIpace1) within[0]);");
            sb.AppendLine("");
            sb.AppendLine("VVIpace1 = ((atomic{senseV -> paceV{SV = 0} -> Skip}) timeout[LRI - URI] ((paceV{SV = 0} -> Skip) within[0]); Wait[URI]; (enableSV{SV = 1} -> VVIpace1) within[0]);");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("SPEC_VVI = paceV -> Loop_VVI;");
            sb.AppendLine("Loop_VVI = (paceV -> Loop_VVI) within[URI, LRI];");
            sb.AppendLine("");
            sb.AppendLine("#assert Sys_VVI refines <T> SPEC_VVI;");
            sb.AppendLine("#assert Sys_VVI deadlockfree;");
            sb.AppendLine("");
            sb.AppendLine("/* VVIR mode */");
            sb.AppendLine("Sys_VVIR = (Heart || VentricleSensor || VVIRpace)\\{senseV, pulseA, pulseV, pulseAmissing,pulseVmissing,enableSV,");
            sb.AppendLine("                               senseNone, senseVLow, senseLOW, senseMEDLOW, senseMED, senseMEDHIGH,");
            sb.AppendLine("                               senseHIGH, senseVHIGH, rateControlling};");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("VVIRpace = ActSensorAndRateControll;VVIRpace1;VVIRpace2;");
            sb.AppendLine("VVIRpace2 = ActSensorAndRateControll;VVIRpace3;VVIRpace2;");
            sb.AppendLine("");
            sb.AppendLine("VVIRpace1 =  ((atomic{senseV -> paceV{SV = 0} -> Skip}) timeout[interval] ((paceV{SV = 0} -> Skip) within[0]); Wait[VRP]; (enableSV{SV = 1} -> Skip) within[0]);");
            sb.AppendLine("");
            sb.AppendLine("VVIRpace3 = ((atomic{senseV -> paceV{SV = 0} -> Skip}) timeout[interval - VRP] ((paceV{SV = 0} -> Skip) within[0]); Wait[VRP]; (enableSV{SV = 1} -> Skip) within[0]);");
            sb.AppendLine("");
            sb.AppendLine("SPEC_VVIR = paceV -> Loop_VVIR;");
            sb.AppendLine("Loop_VVIR = (paceV -> Loop_VVIR) within[interval];");
            sb.AppendLine("");
            sb.AppendLine("#assert Sys_VVIR refines <T> SPEC_VVIR;");
            sb.AppendLine("#assert Sys_VVIR deadlockfree;");
            sb.AppendLine("");
            sb.AppendLine("/* VDD mode */");
            sb.AppendLine("Sys_VDD =  (setSASV{SA = 0; SV = 1;} -> Skip)within[0];( Heart || VentricleSensor || AtriaSensor || VDDpace)");
            sb.AppendLine("                       \\{pulseA, pulseV, pulseAmissing, pulseVmissing, senseV, enableSA, enableSV, setSASV };");
            sb.AppendLine("");
            sb.AppendLine("//sensing both chambers but only pace ventricle,");
            sb.AppendLine("//and a sense in atrium shall cause a ventricular pace after an AV delay,");
            sb.AppendLine("//unless a ventricular sense has been detected before that.");
            sb.AppendLine("VDDpace =  (atomic{senseV -> paceV{SA = 0; SV = 0} -> Skip} timeout[LRI] (paceV{SA = 0; SV = 0} ->Skip) within[0]);");
            sb.AppendLine("                       Wait[URI];(enableSA{SA = 1} -> Skip)within[0];");
            sb.AppendLine("                       (atomic{senseA -> setSASV{SA = 0; SV = 1} -> Skip} timeout[LRI - AVDelay - URI](setSASV{SA = 0; SV = 1} -> Skip)within[0]);");
            sb.AppendLine("                       VDDpace1;");
            sb.AppendLine("");
            sb.AppendLine("VDDpace1 =      (atomic{senseV -> paceV{SA = 0; SV = 0} -> Skip} timeout[AVDelay] (paceV{SA = 0; SV = 0} ->Skip) within[0]);");
            sb.AppendLine("                       Wait[URI];(enableSA{SA = 1} -> Skip)within[0];");
            sb.AppendLine("                       (atomic{senseA -> setSASV{SA = 0; SV = 1} -> Skip} timeout[LRI - AVDelay - URI](setSASV{SA = 0; SV = 1} -> Skip)within[0]);");
            sb.AppendLine("                       VDDpace1;");
            sb.AppendLine("");
            sb.AppendLine("SPEC_VDD = paceV -> Loop_VDD;");
            sb.AppendLine("Loop_VDD = senseA -> Loop_VDDA <> Loop_VDDV;");
            sb.AppendLine("Loop_VDDA = (paceV -> Loop_VDD) within[AVDelay];");
            sb.AppendLine("Loop_VDDV = (paceV -> Loop_VDD) within[URI, LRI];");
            sb.AppendLine("");
            sb.AppendLine("Sys_VDDV = Sys_VDD\\{senseA};");
            sb.AppendLine("SPEC_VDDV = paceV -> Loop_VDDV;");
            sb.AppendLine("");
            sb.AppendLine("#assert Sys_VDD refines <T> SPEC_VDD;");
            sb.AppendLine("#assert Sys_VDDV refines <T> SPEC_VDDV;");
            sb.AppendLine("#assert Sys_VDD deadlockfree;");
            sb.AppendLine("");
            sb.AppendLine("/* VDDR mode */");
            sb.AppendLine("Sys_VDDR =  (setSASV{SA = 0; SV = 1;} -> Skip)within[0];( Heart || VentricleSensor || AtriaSensor || VDDRpace)");
            sb.AppendLine("                       \\{setSASV, pulseA, pulseV, pulseAmissing, pulseVmissing, senseV, enableSA, enableSV,");
            sb.AppendLine("                               senseNone, senseVLow, senseLOW, senseMEDLOW, senseMED, senseMEDHIGH, senseHIGH,");
            sb.AppendLine("                               senseVHIGH, rateControlling };");
            sb.AppendLine("");
            sb.AppendLine("VDDRpace = ActSensorAndRateControll;VDDRpace1;VDDRpace2;");
            sb.AppendLine("VDDRpace2 = ActSensorAndRateControll;VDDRpace3;VDDRpace2;");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("VDDRpace1 = (atomic{senseV -> paceV{SA = 0; SV = 0} -> Skip} timeout[interval] (paceV{SA = 0; SV = 0} ->Skip) within[0]);");
            sb.AppendLine("                       Wait[PVARP];(enableSA{SA = 1} -> Skip)within[0];");
            sb.AppendLine("                       (atomic{senseA -> setSASV{SA = 0; SV = 1} -> Skip} timeout[interval - AVDelay - PVARP](setSASV{SA = 0; SV = 1} -> Skip)within[0]);");
            sb.AppendLine("");
            sb.AppendLine("VDDRpace3 = (atomic{senseV -> paceV{SA = 0; SV = 0} -> Skip} timeout[AVDelay] (paceV{SA = 0; SV = 0} ->Skip) within[0]);");
            sb.AppendLine("                       Wait[PVARP];(enableSA{SA = 1} -> Skip)within[0];");
            sb.AppendLine("                       (atomic{senseA -> setSASV{SA = 0; SV = 1} -> Skip} timeout[interval - AVDelay - PVARP](setSASV{SA = 0; SV = 1} -> Skip)within[0]);");
            sb.AppendLine("");
            sb.AppendLine("SPEC_VDDR = paceV -> Loop_VDDR;");
            sb.AppendLine("Loop_VDDR = senseA -> Loop_VDDRA <> Loop_VDDRV;");
            sb.AppendLine("Loop_VDDRA = (paceV -> Loop_VDDR) within[AVDelay];");
            sb.AppendLine("Loop_VDDRV = (paceV -> Loop_VDDR) within[interval];");
            sb.AppendLine("");
            sb.AppendLine("Sys_VDDRV = Sys_VDDR\\{senseA};");
            sb.AppendLine("SPEC_VDDRV = paceV -> Loop_VDDRV;");
            sb.AppendLine("");
            sb.AppendLine("#assert Sys_VDDR refines <T> SPEC_VDDR;");
            sb.AppendLine("#assert Sys_VDDRV refines <T> SPEC_VDDRV;");
            sb.AppendLine("#assert Sys_VDDR deadlockfree;");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("/* DOO mode*/");
            sb.AppendLine("Sys_DOO = (Heart || DOOpace)\\{pulseA, pulseV, pulseAmissing, pulseVmissing, senseA, senseV, enableSA, enableSV, setSASV };");
            sb.AppendLine("");
            sb.AppendLine("//pace both chambers without sensing");
            sb.AppendLine("DOOpace = (paceV -> Wait[interval - AVDelay])within[0];(paceA ->Wait[AVDelay])within[0];DOOpace;");
            sb.AppendLine("");
            sb.AppendLine("Sys_DOOA = Sys_DOO\\{paceV};");
            sb.AppendLine("SPEC_DOOA = (paceA -> SPEC_DOOA)within[interval];");
            sb.AppendLine("#assert Sys_DOOA refines <T> SPEC_DOOA;");
            sb.AppendLine("");
            sb.AppendLine("Sys_DOOV = Sys_DOO\\{paceA};");
            sb.AppendLine("SPEC_DOOV = (paceV -> SPEC_DOOV)within[interval];");
            sb.AppendLine("#assert Sys_DOOV refines <T> SPEC_DOOV;");
            sb.AppendLine("");
            sb.AppendLine("Sys_DOOAV = Sys_DOO;");
            sb.AppendLine("SPEC_DOOAV = paceV -> Loop_DOOA;");
            sb.AppendLine("Loop_DOOA = paceA -> Loop_DOOV;");
            sb.AppendLine("Loop_DOOV = (paceV -> Loop_DOOA)within[AVDelay];");
            sb.AppendLine("#assert Sys_DOOAV refines <T> SPEC_DOOAV;");
            sb.AppendLine("");
            sb.AppendLine("/* DOOR mode */");
            sb.AppendLine("");
            sb.AppendLine("Sys_DOOR = ( Heart || DOORpace)");
            sb.AppendLine("                               \\{pulseA, pulseV, pulseAmissing, pulseVmissing,");
            sb.AppendLine("                               senseNone, senseVLow, senseLOW, senseMEDLOW, senseMED, senseMEDHIGH, senseHIGH,");
            sb.AppendLine("                               senseVHIGH, rateControlling };");
            sb.AppendLine("");
            sb.AppendLine("DOORpace = ActSensorAndRateControll;(paceV -> Wait[interval - AVDelay])within[0];(paceA ->Wait[AVDelay])within[0];DOORpace;");
            sb.AppendLine("");
            sb.AppendLine("Sys_DOORV = Sys_DOOR\\{paceA};");
            sb.AppendLine("SPEC_DOORV = paceV -> LoopDOORV;");
            sb.AppendLine("LoopDOORV = (paceV -> LoopDOORV)within[interval];");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("Sys_DOORA = Sys_DOOR\\{paceV};");
            sb.AppendLine("SPEC_DOORA = paceA -> LoopDOORA;");
            sb.AppendLine("LoopDOORA = (paceA -> LoopDOORA)within[interval];");
            sb.AppendLine("");
            sb.AppendLine("Sys_DOORAV = Sys_DOOR;");
            sb.AppendLine("SPEC_DOOR = paceV -> SPEC_DOOR1;");
            sb.AppendLine("SPEC_DOOR1 = paceA -> LoopDOOR;");
            sb.AppendLine("LoopDOOR = (paceV -> SPEC_DOOR1)within[AVDelay];");
            sb.AppendLine("");
            sb.AppendLine("#assert Sys_DOORV refines <T> SPEC_DOORV;");
            sb.AppendLine("#assert Sys_DOORA refines <T> SPEC_DOORA;");
            sb.AppendLine("#assert Sys_DOORAV refines <T> SPEC_DOOR ;");
            sb.AppendLine("#assert Sys_DOOR deadlockfree;");
            sb.AppendLine("");
            sb.AppendLine("/* DDI mode */");
            sb.AppendLine("Sys_DDI=  (setSASV{SA = 0; SV = 1;} -> Skip)within[0];( Heart || VentricleSensor || AtriaSensor || DDIpace)");
            sb.AppendLine("                       \\{pulseA, pulseV, pulseAmissing, pulseVmissing, senseA, senseV, enableSA, enableSV, setSASV };");
            sb.AppendLine("");
            sb.AppendLine("//sensing and pacing both chambers, and a sense from a chamber will inhit the pending pace in that chamber");
            sb.AppendLine("DDIpace =  (atomic{senseV -> paceV{SA = 0; SV = 0} -> Skip} timeout[LRI] (paceV{SA = 0; SV = 0} ->Skip) within[0]);");
            sb.AppendLine("                       Wait[URI];(enableSA{SA = 1} -> Skip)within[0];");
            sb.AppendLine("                       (atomic{senseA -> paceA{SA = 0; SV = 1} -> Skip} timeout[LRI - AVDelay - URI](paceA{SA = 0; SV = 1} -> Skip)within[0]);");
            sb.AppendLine("                       DDIpace1;");
            sb.AppendLine("");
            sb.AppendLine("DDIpace1 =      (atomic{senseV -> paceV{SA = 0; SV = 0} -> Skip} timeout[AVDelay] (paceV{SA = 0; SV = 0} ->Skip) within[0]);");
            sb.AppendLine("                       Wait[URI];(enableSA{SA = 1} -> Skip)within[0];");
            sb.AppendLine("                       (atomic{senseA -> paceA{SA = 0; SV = 1} -> Skip} timeout[LRI - AVDelay - URI](paceA{SA = 0; SV = 1} -> Skip)within[0]);");
            sb.AppendLine("                       DDIpace1;");
            sb.AppendLine("");
            sb.AppendLine("Sys_DDIV = Sys_DDI\\{paceA};");
            sb.AppendLine("SPEC_DDIV = paceV -> LoopDDIV;");
            sb.AppendLine("LoopDDIV = (paceV -> LoopDDIV)within[URI,LRI];");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("Sys_DDIA = Sys_DDI\\{paceV};");
            sb.AppendLine("SPEC_DDIA = paceA -> LoopDDIA;");
            sb.AppendLine("LoopDDIA = (paceA -> LoopDDIA)within[URI,LRI];");
            sb.AppendLine("");
            sb.AppendLine("Sys_DDIAV = Sys_DDI;");
            sb.AppendLine("SPEC_DDI = paceV -> SPEC_DDI1;");
            sb.AppendLine("SPEC_DDI1 = paceA -> LoopDDI;");
            sb.AppendLine("LoopDDI = (paceV -> SPEC_DDI1)within[AVDelay];");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("#assert Sys_DDIV refines <T> SPEC_DDIV;");
            sb.AppendLine("#assert Sys_DDIA refines <T> SPEC_DDIA;");
            sb.AppendLine("#assert Sys_DDIAV  refines <T> SPEC_DDI ;");
            sb.AppendLine("#assert Sys_DDI deadlockfree;");
            sb.AppendLine("");
            sb.AppendLine("/* DDIR mode */");
            sb.AppendLine("");
            sb.AppendLine("Sys_DDIR = (setSASV{SA = 0; SV = 1;} -> Skip)within[0];( Heart || VentricleSensor || AtriaSensor || DDIRpace)");
            sb.AppendLine("                               \\{setSASV, pulseA, pulseV, pulseAmissing, pulseVmissing, senseA, senseV, enableSA, enableSV,");
            sb.AppendLine("                               senseNone, senseVLow, senseLOW, senseMEDLOW, senseMED, senseMEDHIGH, senseHIGH,");
            sb.AppendLine("                               senseVHIGH, rateControlling };");
            sb.AppendLine("");
            sb.AppendLine("DDIRpace = ActSensorAndRateControll;DDIRpace2;DDIRpace1;");
            sb.AppendLine("DDIRpace1 = ActSensorAndRateControll;DDIRpace3;DDIRpace1;");
            sb.AppendLine("");
            sb.AppendLine("DDIRpace2 = (atomic{senseV -> paceV{SA = 0; SV = 0} -> Skip} timeout[interval] (paceV{SA = 0; SV = 0} ->Skip) within[0]);");
            sb.AppendLine("                       Wait[PVARP];(enableSA{SA = 1} -> Skip)within[0];");
            sb.AppendLine("                       (atomic{senseA -> paceA{SA = 0; SV = 1} -> Skip} timeout[interval - AVDelay - PVARP](paceA{SA = 0; SV = 1} -> Skip)within[0]);");
            sb.AppendLine("");
            sb.AppendLine("DDIRpace3 = (atomic{senseV -> paceV{SA = 0; SV = 0} -> Skip} timeout[AVDelay] (paceV{SA = 0; SV = 0} ->Skip) within[0]);");
            sb.AppendLine("                       Wait[PVARP];(enableSA{SA = 1} -> Skip)within[0];");
            sb.AppendLine("                       (atomic{senseA -> paceA{SA = 0; SV = 1} -> Skip} timeout[interval - AVDelay - PVARP](paceA{SA = 0; SV = 1} -> Skip)within[0]);");
            sb.AppendLine("");
            sb.AppendLine("Sys_DDIRV = Sys_DDIR\\{paceA};");
            sb.AppendLine("SPEC_DDIRV = paceV -> LoopDDIRV;");
            sb.AppendLine("LoopDDIRV = (paceV -> LoopDDIRV)within[interval];");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("Sys_DDIRA = Sys_DDIR\\{paceV};");
            sb.AppendLine("SPEC_DDIRA = paceA -> LoopDDIRA;");
            sb.AppendLine("LoopDDIRA = (paceA -> LoopDDIRA)within[interval];");
            sb.AppendLine("");
            sb.AppendLine("Sys_DDIRAV = Sys_DDIR;");
            sb.AppendLine("SPEC_DDIR = paceV -> SPEC_DDIR1;");
            sb.AppendLine("SPEC_DDIR1 = paceA -> LoopDDIR;");
            sb.AppendLine("LoopDDIR = (paceV -> SPEC_DDIR1)within[AVDelay];");
            sb.AppendLine("");
            sb.AppendLine("#assert Sys_DDIRV refines <T> SPEC_DDIRV;");
            sb.AppendLine("#assert Sys_DDIRA refines <T> SPEC_DDIRA;");
            sb.AppendLine("#assert Sys_DDIRAV refines <T> SPEC_DDIR ;");
            sb.AppendLine("#assert Sys_DDIR deadlockfree;");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("/* DDD mode */");
            sb.AppendLine("Sys_DDD =  (setSASV{SA = 0; SV = 1;} -> Skip)within[0];( Heart || VentricleSensor || AtriaSensor || DDDpace)");
            sb.AppendLine("                       \\{pulseA, pulseV, pulseAmissing, pulseVmissing, senseA, senseV, enableSA, enableSV, setSASV };");
            sb.AppendLine("");
            sb.AppendLine("//sensing and pacing both chambers, and an atrial event will begin and AV delay interval event.");
            sb.AppendLine("DDDpace =  (atomic{senseV -> paceV{SA = 0; SV = 0} -> Skip} timeout[LRI] (paceV{SA = 0; SV = 0} ->Skip) within[0]);");
            sb.AppendLine("                       Wait[URI];(enableSA{SA = 1} -> Skip)within[0];");
            sb.AppendLine("                       (atomic{senseA -> paceA{SA = 0; SV = 1} -> Skip} timeout[LRI - AVDelay - URI](paceA{SA = 0; SV = 1} -> Skip)within[0]);");
            sb.AppendLine("                       DDDpace1;");
            sb.AppendLine("");
            sb.AppendLine("DDDpace1 =      (atomic{senseV -> paceV{SA = 0; SV = 0} -> Skip} timeout[AVDelay] (paceV{SA = 0; SV = 0} ->Skip) within[0]);");
            sb.AppendLine("                       Wait[URI];(enableSA{SA = 1} -> Skip)within[0];");
            sb.AppendLine("                       (atomic{senseA -> paceA{SA = 0; SV = 1} -> Skip} timeout[LRI - AVDelay - URI](paceA{SA = 0; SV = 1} -> Skip)within[0]);");
            sb.AppendLine("                       DDDpace1;");
            sb.AppendLine("");
            sb.AppendLine("Sys_DDDV = Sys_DDD\\{paceA};");
            sb.AppendLine("SPEC_DDDV = paceV -> LoopDDDV;");
            sb.AppendLine("LoopDDDV = (paceV -> LoopDDDV)within[URI,LRI];");
            sb.AppendLine("");
            sb.AppendLine("Sys_DDDA = Sys_DDD\\{paceV};");
            sb.AppendLine("SPEC_DDDA = paceA -> LoopDDDA;");
            sb.AppendLine("LoopDDDA = (paceA -> LoopDDDA)within[URI,LRI];");
            sb.AppendLine("");
            sb.AppendLine("Sys_DDDAV = Sys_DDD;");
            sb.AppendLine("SPEC_DDD = paceV -> SPEC_DDD1;");
            sb.AppendLine("SPEC_DDD1 = paceA -> LoopDDD;");
            sb.AppendLine("LoopDDD = (paceV -> SPEC_DDD1)within[AVDelay];");
            sb.AppendLine("");
            sb.AppendLine("#assert Sys_DDDV refines <T> SPEC_DDDV;");
            sb.AppendLine("#assert Sys_DDDA refines <T> SPEC_DDDA;");
            sb.AppendLine("#assert Sys_DDDAV  refines <T> SPEC_DDD ;");
            sb.AppendLine("#assert Sys_DDD deadlockfree;");
            sb.AppendLine("");
            sb.AppendLine("/*DDDR mode*/");
            sb.AppendLine("Sys_DDDR = (setSASV{SA = 0; SV = 1;} -> Skip)within[0];( Heart || VentricleSensor || AtriaSensor || DDDRpace)");
            sb.AppendLine("                               \\{setSASV, pulseA, pulseV, pulseAmissing, pulseVmissing, senseA, senseV, enableSA, enableSV,");
            sb.AppendLine("                               senseNone, senseVLow, senseLOW, senseMEDLOW, senseMED, senseMEDHIGH, senseHIGH,");
            sb.AppendLine("                               senseVHIGH, rateControlling };");
            sb.AppendLine("");
            sb.AppendLine("DDDRpace = ActSensorAndRateControll;DDDRpace1;DDDRpace2;");
            sb.AppendLine("DDDRpace2 = ActSensorAndRateControll;DDDRpace3;DDDRpace2;");
            sb.AppendLine("");
            sb.AppendLine("DDDRpace1 = (atomic{senseV -> paceV{SA = 0; SV = 0} -> Skip} timeout[interval] (paceV{SA = 0; SV = 0} ->Skip) within[0]);");
            sb.AppendLine("                       Wait[PVARP];(enableSA{SA = 1} -> Skip)within[0];");
            sb.AppendLine("                       (atomic{senseA -> paceA{SA = 0; SV = 1} -> Skip} timeout[interval - AVDelay - PVARP](paceA{SA = 0; SV = 1} -> Skip)within[0]);");
            sb.AppendLine("");
            sb.AppendLine("DDDRpace3 = (atomic{senseV -> paceV{SA = 0; SV = 0} -> Skip} timeout[AVDelay] (paceV{SA = 0; SV = 0} ->Skip) within[0]);");
            sb.AppendLine("                       Wait[PVARP];(enableSA{SA = 1} -> Skip)within[0];");
            sb.AppendLine("                       (atomic{senseA -> paceA{SA = 0; SV = 1} -> Skip} timeout[interval - AVDelay - PVARP](paceA{SA = 0; SV = 1} -> Skip)within[0]);");
            sb.AppendLine("");
            sb.AppendLine("Sys_DDDRV = Sys_DDDR\\{paceA};");
            sb.AppendLine("SPEC_DDDRV = paceV -> LoopDDDRV;");
            sb.AppendLine("LoopDDDRV = (paceV -> LoopDDDRV)within[interval];");
            sb.AppendLine("");
            sb.AppendLine("Sys_DDDRA = Sys_DDDR\\{paceV};");
            sb.AppendLine("SPEC_DDDRA = paceA -> LoopDDDRA;");
            sb.AppendLine("LoopDDDRA = (paceA -> LoopDDDRA)within[interval];");
            sb.AppendLine("");
            sb.AppendLine("Sys_DDDRAV = Sys_DDDR;");
            sb.AppendLine("SPEC_DDDR = paceV -> SPEC_DDDR1;");
            sb.AppendLine("SPEC_DDDR1 = paceA -> LoopDDDR;");
            sb.AppendLine("LoopDDDR = (paceV -> SPEC_DDDR1)within[AVDelay];");
            sb.AppendLine("");
            sb.AppendLine("#assert Sys_DDDRV refines <T> SPEC_DDDRV;");
            sb.AppendLine("#assert Sys_DDDRA refines <T> SPEC_DDDRA;");
            sb.AppendLine("#assert Sys_DDDRAV refines <T> SPEC_DDDR ;");
            sb.AppendLine("#assert Sys_DDDR deadlockfree;");

            return sb.ToString();

        }

        public static string LoadLightControlSystem()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("//Light Control System");
            sb.AppendLine("");
            sb.AppendLine("var dim : {0..100};");
            sb.AppendLine("var on = false;");
            sb.AppendLine("channel button 0;");
            sb.AppendLine("channel dimmer 0;");
            sb.AppendLine("channel motion 0;");
            sb.AppendLine("");
            sb.AppendLine("//the button class");
            sb.AppendLine("TurningOn = turnOn{on=true;dim=100} -> Skip;");
            sb.AppendLine("TurningOff = turnOff{on=false;dim=0} -> Skip;");
            sb.AppendLine("");
            sb.AppendLine("//the light class");
            sb.AppendLine("ButtonPushing = button?1 ->");
            sb.AppendLine("       if (dim > 0) {");
            sb.AppendLine("               TurningOn");
            sb.AppendLine("       }");
            sb.AppendLine("       else {");
            sb.AppendLine("               TurningOff");
            sb.AppendLine("       };");
            sb.AppendLine("");
            sb.AppendLine("DimChange = dimmer?n ->");
            sb.AppendLine("       if (on) {");
            sb.AppendLine("               setdim{dim = n} -> Skip");
            sb.AppendLine("       }");
            sb.AppendLine("       else {");
            sb.AppendLine("               Skip");
            sb.AppendLine("       };");
            sb.AppendLine("");
            sb.AppendLine("ControlledLight = (ButtonPushing [] DimChange); ControlledLight;");
            sb.AppendLine("");
            sb.AppendLine("//the motion detector");
            sb.AppendLine("NoUser = move -> motion!1 -> User [] nomove -> Wait[1]; NoUser;");
            sb.AppendLine("User = nomove -> motion!0 -> NoUser [] move -> Wait[1]; User;");
            sb.AppendLine("MotionDetector = NoUser;");
            sb.AppendLine("");
            sb.AppendLine("//the room controller");
            sb.AppendLine("Ready = motion?1 -> button!1 -> On;");
            sb.AppendLine("Regular = adjust -> dimmer!50 -> Regular;");
            sb.AppendLine("On = Regular interrupt motion?0 -> OnAgain;");
            sb.AppendLine("OnAgain = (motion?1 -> On) timeout[20] Off;");
            sb.AppendLine("Off = dimmer!0 -> Ready;");
            sb.AppendLine("Controller = Off;");
            sb.AppendLine("");
            sb.AppendLine("//the system");
            sb.AppendLine("System = MotionDetector ||| ControlledLight ||| Controller;");
            sb.AppendLine("");
            sb.AppendLine("#assert System deadlockfree;");
            sb.AppendLine("#assert System divergencefree;");
            sb.AppendLine("");
            sb.AppendLine("#define inv ((on && dim > 0) || (!on && dim == 0));");
            sb.AppendLine("#assert System |= []inv;");
            sb.AppendLine("");
            sb.AppendLine("#assert System |= [](turnOn -> <> turnOff);");
            return sb.ToString();
        }


        public static string LoadABPSystem(int N)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("//=======================Model Details===========================");
            sb.AppendLine("#define CHANNELSIZE " + N + ";");
            sb.AppendLine("");
            sb.AppendLine("channel c CHANNELSIZE; //unreliable channel.");
            sb.AppendLine("channel d CHANNELSIZE; //perfect channel.");
            sb.AppendLine("channel tmr 0; //a synchronous channel between sender and timer, which is used to implement premature timeout.");
            sb.AppendLine("");
            sb.AppendLine("Sender(alterbit) = (c!alterbit -> Skip [] lost -> Skip);");
            sb.AppendLine("                                  tmr!1 -> Wait4Response(alterbit);");
            sb.AppendLine("");
            sb.AppendLine("Wait4Response(alterbit) = (d?x -> ifa (x==alterbit) {");
            sb.AppendLine("                                      tmr!0 -> Sender(1-alterbit)");
            sb.AppendLine("                                  } else {");
            sb.AppendLine("                                      Wait4Response(alterbit)");
            sb.AppendLine("                                  })");
            sb.AppendLine("                          [] tmr?2 -> Sender(alterbit);");
            sb.AppendLine("");
            sb.AppendLine("Receiver(alterbit) = c?x -> ifa (x==alterbit) {");
            sb.AppendLine("                                 d!alterbit -> Receiver(1-alterbit)");
            sb.AppendLine("                            } else {");
            sb.AppendLine("                                 Receiver(alterbit)");
            sb.AppendLine("                            };");
            sb.AppendLine("");
            sb.AppendLine("Timer = tmr?1 -> (tmr?0 -> Timer [] tmr!2 -> Timer);");
            sb.AppendLine("");
            sb.AppendLine("ABP = Sender(0) ||| Receiver(0) ||| Timer;");
            sb.AppendLine("");
            sb.AppendLine("#assert ABP deadlockfree;");
            sb.AppendLine("#assert ABP |= []<> lost;");
            return sb.ToString();
        }

        public static string LoadABPTimedSystem(int N)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("//=======================Model Details===========================");
            sb.AppendLine("#define CHANNELSIZE " + N + ";");
            sb.AppendLine("#define TO 2;");
            sb.AppendLine("channel c CHANNELSIZE; //unreliable channel.");
            sb.AppendLine("channel d CHANNELSIZE; //perfect channel.");
            sb.AppendLine("");
            sb.AppendLine("Sender(alterbit) = (c!alterbit -> Skip [] lost -> Skip);");
            sb.AppendLine("                   ((Wait4Response(alterbit) deadline[TO]; Sender(1-alterbit))");
            sb.AppendLine("                   [] ");
            sb.AppendLine("                   (Wait[TO]; Sender(alterbit)));");
            sb.AppendLine("");
            sb.AppendLine("Wait4Response(alterbit) = d?x -> if (x!=alterbit) {Wait4Response(alterbit)}; ");
            sb.AppendLine("");
            sb.AppendLine("Receiver(alterbit) = c?x ->");
            sb.AppendLine("                     if (x==alterbit) {");
            sb.AppendLine("                           d!alterbit -> Receiver(1-alterbit)");
            sb.AppendLine("                     } else {");
            sb.AppendLine("                           Receiver(alterbit)");
            sb.AppendLine("                     };");
            sb.AppendLine("");
            sb.AppendLine("ABP = Sender(0) ||| Receiver(0);");
            sb.AppendLine("");
            sb.AppendLine("#assert ABP deadlockfree;");
            sb.AppendLine("#assert ABP |= []<> lost;"); 
            return sb.ToString();
        }


        public static string LoadHanoiTower(int n)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("#define N " + n + ";");
            sb.AppendLine("var column1[N+1];");
            sb.AppendLine("var column2[N+1];");
            sb.AppendLine("var column3[N+1];");
            sb.AppendLine("var size[4];");
            sb.AppendLine();
            sb.AppendLine("Init()=ini{size[1]=N;size[2]=0;size[3]=0;" + LoadHanoiTowerUltil1(1, n) + "}->Skip;");

            sb.AppendLine("Move1To2()=if((size[1] > 0&&column1[size[1]]<column2[size[2]])||(size[1] >0&&size[2]==0))");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tmove.1.2{size[2]=size[2]+1;column2[size[2]]=column1[size[1]];size[1]=size[1]-1;}->Skip");
            sb.AppendLine("\t};");

            sb.AppendLine("Move1To3()=if((size[1] > 0&&column1[size[1]]<column3[size[3]])||(size[1] >0&&size[3]==0))");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tmove.1.3{size[3]=size[3]+1;column3[size[3]]=column1[size[1]];size[1]=size[1]-1;}->Skip");
            sb.AppendLine("\t};");

            sb.AppendLine("Move2To1()=if((size[2] > 0&&column2[size[2]]<column1[size[1]])||(size[2] >0&&size[1]==0))");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tmove.2.1{size[1]=size[1]+1;column1[size[1]]=column2[size[2]];size[2]=size[2]-1;}->Skip");
            sb.AppendLine("\t};");

            sb.AppendLine("Move2To3()=if((size[2] > 0&&column2[size[2]]<column3[size[3]])||(size[2] >0&&size[3]==0))");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tmove.2.3{size[3]=size[3]+1;column3[size[3]]=column2[size[2]];size[2]=size[2]-1;}->Skip");
            sb.AppendLine("\t};");

            sb.AppendLine("Move3To1()=if((size[3] > 0&&column3[size[3]]<column1[size[1]])||(size[3] >0&&size[1]==0))");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tmove.3.1{size[1]=size[1]+1;column1[size[1]]=column3[size[3]];size[3]=size[3]-1;}->Skip");
            sb.AppendLine("\t};");

            sb.AppendLine("Move3To2()=if((size[3] >0&&column3[size[3]]<column2[size[2]])||(size[3] >0&&size[2]==0))");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tmove.3.2{size[2]=size[2]+1;column2[size[2]]=column3[size[3]];size[3]=size[3]-1;}->Skip");
            sb.AppendLine("\t};");

            sb.AppendLine("Move()=	Move1To2()[]Move1To3()[]Move2To1()[]Move2To3()[]Move3To1()[]Move3To2();");
            sb.AppendLine("System()=Init();System1();");
            sb.AppendLine("System1()=Move();System1();");
            sb.AppendLine("#define goal (" + LoadHanoiTowerUltil2(3, n) + ");");
            sb.AppendLine("#assert System() reaches goal;");

            return sb.ToString();
        }
        private static string LoadHanoiTowerUltil1(int column, int N)
        {
            string result = string.Empty;
            for (int i = 1; i <= N; i++)
            {
                result += "column" + column + "[" + i + "]=" + (N + 1 - i) + ";";
            }
            return result;
        }
        private static string LoadHanoiTowerUltil2(int column, int N)
        {
            string result = string.Empty;
            for (int i = 1; i <= N; i++)
            {
                result += "column" + column + "[" + i + "]==" + (N + 1 - i) + "&&";
            }
            result = result.TrimEnd('&');
            return result;
        }

        public static string LoadRubiksCube()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("#define white 1;");
            sb.AppendLine("#define yellow 2;");
            sb.AppendLine("#define red 3;");
            sb.AppendLine("#define blue 4;");
            sb.AppendLine("#define green 5;");
            sb.AppendLine("#define orange 6;");
            sb.AppendLine("");
            sb.AppendLine("//this model models the initial configuration ");
            sb.AppendLine("var Cubix = [			blue, 	yellow, blue, ");
            sb.AppendLine("						green, 	green, 	green,");
            sb.AppendLine("						red, 	yellow, yellow,");
            sb.AppendLine("red, 	blue, 	green,	white,	red,	orange,	blue,	orange,	orange,");
            sb.AppendLine("blue, 	blue, 	blue,	orange,	orange,	white,	orange,	white,	red,");
            sb.AppendLine("orange, blue, 	white,	orange,	white,	red,	yellow,	red,	orange,");
            sb.AppendLine("						green, 	green, 	white, ");
            sb.AppendLine("						yellow, yellow, white,");
            sb.AppendLine("						white, 	yellow, blue,");
            sb.AppendLine("						yellow, white, 	red, ");
            sb.AppendLine("						red, 	red, 	green,");
            sb.AppendLine("						green, 	orange, green];");
            sb.AppendLine("var CubixPrime = [		blue, 	yellow, blue, ");
            sb.AppendLine("						green, 	green, 	green,");
            sb.AppendLine("						red, 	yellow, yellow,");
            sb.AppendLine("red, 	blue, 	green,	white,	red,	orange,	blue,	orange,	orange,");
            sb.AppendLine("blue, 	blue, 	blue,	orange,	orange,	white,	orange,	white,	red,");
            sb.AppendLine("orange, blue, 	white,	orange,	white,	red,	yellow,	red,	orange,");
            sb.AppendLine("						green, 	green, 	white, ");
            sb.AppendLine("						yellow, yellow, white,");
            sb.AppendLine("						white, 	yellow, blue,");
            sb.AppendLine("						yellow, white, 	red, ");
            sb.AppendLine("						red, 	red, 	green,");
            sb.AppendLine("						green, 	orange, green];");
            sb.AppendLine("");
            sb.AppendLine("LeftTop() = turn_top_row_left{");
            sb.AppendLine("				//the top face");
            sb.AppendLine("				CubixPrime[0] = Cubix[6]; CubixPrime[1] = Cubix[3];	CubixPrime[2] = Cubix[0];");
            sb.AppendLine("				CubixPrime[3] = Cubix[7]; CubixPrime[5] = Cubix[1]; CubixPrime[6] = Cubix[8];");
            sb.AppendLine("				CubixPrime[7] = Cubix[5]; CubixPrime[8] = Cubix[2];");
            sb.AppendLine("				//the side face");
            sb.AppendLine("				CubixPrime[9] = Cubix[12]; CubixPrime[10] = Cubix[13];");
            sb.AppendLine("				CubixPrime[11] = Cubix[14]; CubixPrime[12] = Cubix[15];");
            sb.AppendLine("				CubixPrime[13] = Cubix[16]; CubixPrime[14] = Cubix[17];");
            sb.AppendLine("				CubixPrime[15] = Cubix[53]; CubixPrime[16] = Cubix[52];");
            sb.AppendLine("				CubixPrime[17] = Cubix[51]; CubixPrime[51] = Cubix[11];");
            sb.AppendLine("				CubixPrime[52] = Cubix[10]; CubixPrime[53] = Cubix[9];");
            sb.AppendLine("				Cubix = CubixPrime;");
            sb.AppendLine("			} -> Game();");
            sb.AppendLine("LeftMiddle() = turn_middle_row_left{");
            sb.AppendLine("				//the side face");
            sb.AppendLine("				CubixPrime[18] = Cubix[21]; CubixPrime[19] = Cubix[22];");
            sb.AppendLine("				CubixPrime[20] = Cubix[23]; CubixPrime[21] = Cubix[24];");
            sb.AppendLine("				CubixPrime[22] = Cubix[25]; CubixPrime[23] = Cubix[26];");
            sb.AppendLine("				CubixPrime[24] = Cubix[50]; CubixPrime[25] = Cubix[49];");
            sb.AppendLine("				CubixPrime[26] = Cubix[48]; CubixPrime[48] = Cubix[20];");
            sb.AppendLine("				CubixPrime[49] = Cubix[19]; CubixPrime[50] = Cubix[18];");
            sb.AppendLine("				Cubix = CubixPrime;");
            sb.AppendLine("			} -> Game();");
            sb.AppendLine("");
            sb.AppendLine("LeftBottom() = turn_bottom_row_left{");
            sb.AppendLine("				//the bottom face");
            sb.AppendLine("				CubixPrime[36] = Cubix[38]; CubixPrime[37] = Cubix[41];	CubixPrime[38] = Cubix[44];");
            sb.AppendLine("				CubixPrime[39] = Cubix[37]; CubixPrime[41] = Cubix[43]; CubixPrime[42] = Cubix[36];");
            sb.AppendLine("				CubixPrime[43] = Cubix[39]; CubixPrime[44] = Cubix[42];");
            sb.AppendLine("				//the side face");
            sb.AppendLine("				CubixPrime[27] = Cubix[30]; CubixPrime[28] = Cubix[31];");
            sb.AppendLine("				CubixPrime[29] = Cubix[32]; CubixPrime[30] = Cubix[33];");
            sb.AppendLine("				CubixPrime[31] = Cubix[34]; CubixPrime[32] = Cubix[35];");
            sb.AppendLine("				CubixPrime[33] = Cubix[47]; CubixPrime[34] = Cubix[46];");
            sb.AppendLine("				CubixPrime[35] = Cubix[45]; CubixPrime[45] = Cubix[29];");
            sb.AppendLine("				CubixPrime[46] = Cubix[28]; CubixPrime[47] = Cubix[27];");
            sb.AppendLine("				Cubix = CubixPrime;");
            sb.AppendLine("			} -> Game();");
            sb.AppendLine("");
            sb.AppendLine("LeftUp() = turn_left_column_up{");
            sb.AppendLine("				//the side face");
            sb.AppendLine("				CubixPrime[9] = Cubix[11]; CubixPrime[10] = Cubix[20];	CubixPrime[11] = Cubix[29];");
            sb.AppendLine("				CubixPrime[18] = Cubix[10]; CubixPrime[20] = Cubix[28]; CubixPrime[27] = Cubix[9];");
            sb.AppendLine("				CubixPrime[28] = Cubix[18]; CubixPrime[29] = Cubix[27];");
            sb.AppendLine("				//the side face");
            sb.AppendLine("				CubixPrime[0] = Cubix[12]; CubixPrime[3] = Cubix[21];");
            sb.AppendLine("				CubixPrime[6] = Cubix[30]; CubixPrime[12] = Cubix[36];");
            sb.AppendLine("				CubixPrime[21] = Cubix[39]; CubixPrime[30] = Cubix[42];");
            sb.AppendLine("				CubixPrime[36] = Cubix[45]; CubixPrime[39] = Cubix[48];");
            sb.AppendLine("				CubixPrime[42] = Cubix[51]; CubixPrime[45] = Cubix[0];");
            sb.AppendLine("				CubixPrime[48] = Cubix[3]; CubixPrime[51] = Cubix[6];");
            sb.AppendLine("				Cubix = CubixPrime;");
            sb.AppendLine("			} -> Game();");
            sb.AppendLine("");
            sb.AppendLine("MiddleUp() = turn_middle_column_up{");
            sb.AppendLine("				//the side face");
            sb.AppendLine("				CubixPrime[1] = Cubix[13]; CubixPrime[4] = Cubix[22];");
            sb.AppendLine("				CubixPrime[7] = Cubix[31]; CubixPrime[13] = Cubix[37];");
            sb.AppendLine("				CubixPrime[22] = Cubix[40]; CubixPrime[31] = Cubix[43];");
            sb.AppendLine("				CubixPrime[37] = Cubix[46]; CubixPrime[40] = Cubix[49];");
            sb.AppendLine("				CubixPrime[43] = Cubix[52]; CubixPrime[46] = Cubix[1];");
            sb.AppendLine("				CubixPrime[49] = Cubix[4]; CubixPrime[52] = Cubix[7];");
            sb.AppendLine("				Cubix = CubixPrime;");
            sb.AppendLine("			} -> Game();");
            sb.AppendLine("");
            sb.AppendLine("RightUp() = turn_right_column_up{");
            sb.AppendLine("				//the side face");
            sb.AppendLine("				CubixPrime[15] = Cubix[33]; CubixPrime[16] = Cubix[24];	CubixPrime[17] = Cubix[15];");
            sb.AppendLine("				CubixPrime[24] = Cubix[34]; CubixPrime[26] = Cubix[16]; CubixPrime[33] = Cubix[35];");
            sb.AppendLine("				CubixPrime[34] = Cubix[26]; CubixPrime[35] = Cubix[17];");
            sb.AppendLine("				//the side face");
            sb.AppendLine("				CubixPrime[2] = Cubix[14]; CubixPrime[5] = Cubix[23];");
            sb.AppendLine("				CubixPrime[8] = Cubix[32]; CubixPrime[14] = Cubix[38];");
            sb.AppendLine("				CubixPrime[23] = Cubix[41]; CubixPrime[32] = Cubix[44];");
            sb.AppendLine("				CubixPrime[38] = Cubix[47]; CubixPrime[41] = Cubix[50];");
            sb.AppendLine("				CubixPrime[44] = Cubix[53]; CubixPrime[47] = Cubix[2];");
            sb.AppendLine("				CubixPrime[50] = Cubix[5]; CubixPrime[53] = Cubix[8];");
            sb.AppendLine("				Cubix = CubixPrime;");
            sb.AppendLine("			} -> Game();");
            sb.AppendLine("");
            sb.AppendLine("LeftBack() = turn_back_left{");
            sb.AppendLine("				//the bottom face");
            sb.AppendLine("				CubixPrime[51] = Cubix[53]; CubixPrime[52] = Cubix[50];	CubixPrime[53] = Cubix[47];");
            sb.AppendLine("				CubixPrime[48] = Cubix[52]; CubixPrime[50] = Cubix[46]; CubixPrime[45] = Cubix[51];");
            sb.AppendLine("				CubixPrime[46] = Cubix[48]; CubixPrime[47] = Cubix[45];");
            sb.AppendLine("				//the side face");
            sb.AppendLine("				CubixPrime[0] = Cubix[17]; CubixPrime[1] = Cubix[26];");
            sb.AppendLine("				CubixPrime[2] = Cubix[35]; CubixPrime[17] = Cubix[44];");
            sb.AppendLine("				CubixPrime[26] = Cubix[43]; CubixPrime[35] = Cubix[42];");
            sb.AppendLine("				CubixPrime[44] = Cubix[27]; CubixPrime[43] = Cubix[18];");
            sb.AppendLine("				CubixPrime[42] = Cubix[9]; CubixPrime[27] = Cubix[0];");
            sb.AppendLine("				CubixPrime[18] = Cubix[1]; CubixPrime[9] = Cubix[2];");
            sb.AppendLine("				Cubix = CubixPrime;");
            sb.AppendLine("			} -> Game();");
            sb.AppendLine("");
            sb.AppendLine("LeftMiddleBack() = turn_middle_back_left{");
            sb.AppendLine("				//the side face");
            sb.AppendLine("				CubixPrime[3] = Cubix[16]; CubixPrime[4] = Cubix[25];");
            sb.AppendLine("				CubixPrime[5] = Cubix[34]; CubixPrime[16] = Cubix[41];");
            sb.AppendLine("				CubixPrime[25] = Cubix[40]; CubixPrime[34] = Cubix[39];");
            sb.AppendLine("				CubixPrime[41] = Cubix[28]; CubixPrime[40] = Cubix[19];");
            sb.AppendLine("				CubixPrime[39] = Cubix[10]; CubixPrime[28] = Cubix[3];");
            sb.AppendLine("				CubixPrime[19] = Cubix[4]; CubixPrime[10] = Cubix[5];");
            sb.AppendLine("				Cubix = CubixPrime;");
            sb.AppendLine("			} -> Game();");
            sb.AppendLine("");
            sb.AppendLine("LeftFrontBack() = turn_front_back_left{");
            sb.AppendLine("				//the front face");
            sb.AppendLine("				CubixPrime[12] = Cubix[14]; CubixPrime[13] = Cubix[23];	CubixPrime[14] = Cubix[32];");
            sb.AppendLine("				CubixPrime[21] = Cubix[13]; CubixPrime[23] = Cubix[31]; CubixPrime[30] = Cubix[12];");
            sb.AppendLine("				CubixPrime[31] = Cubix[21]; CubixPrime[32] = Cubix[30];");
            sb.AppendLine("				//the side face");
            sb.AppendLine("				CubixPrime[6] = Cubix[15]; CubixPrime[7] = Cubix[24];");
            sb.AppendLine("				CubixPrime[8] = Cubix[33]; CubixPrime[15] = Cubix[38];");
            sb.AppendLine("				CubixPrime[24] = Cubix[37]; CubixPrime[33] = Cubix[36];");
            sb.AppendLine("				CubixPrime[38] = Cubix[29]; CubixPrime[37] = Cubix[20];");
            sb.AppendLine("				CubixPrime[36] = Cubix[11]; CubixPrime[29] = Cubix[6];");
            sb.AppendLine("				CubixPrime[20] = Cubix[7]; CubixPrime[11] = Cubix[8];");
            sb.AppendLine("				Cubix = CubixPrime;");
            sb.AppendLine("			} -> Game();");
            sb.AppendLine("");
            sb.AppendLine("Game() = LeftTop() [] ");
            sb.AppendLine("		LeftMiddle() []");
            sb.AppendLine("		LeftBottom() []");
            sb.AppendLine("		LeftUp() []");
            sb.AppendLine("		MiddleUp() []");
            sb.AppendLine("		RightUp() []");
            sb.AppendLine("		LeftBack() []");
            sb.AppendLine("		LeftMiddleBack() []");
            sb.AppendLine("		LeftFrontBack(); ");
            sb.AppendLine("		");
            sb.AppendLine("#define goal Cubix[0] == Cubix[1] && Cubix[1] == Cubix[2] && Cubix[2] == Cubix[3] && ");
            sb.AppendLine("			 Cubix[3] == Cubix[4] && Cubix[4] == Cubix[5] && Cubix[5] == Cubix[6] &&   ");
            sb.AppendLine("			 Cubix[6] == Cubix[7] && Cubix[7] == Cubix[8] &&   ");
            sb.AppendLine("			 Cubix[9] == Cubix[10] && Cubix[10] == Cubix[11] && Cubix[11] == Cubix[18] && ");
            sb.AppendLine("			 Cubix[18] == Cubix[19] && Cubix[19] == Cubix[20] && Cubix[20] == Cubix[27] &&   ");
            sb.AppendLine("			 Cubix[27] == Cubix[28] && Cubix[28] == Cubix[29] &&			 ");
            sb.AppendLine("			 Cubix[12] == Cubix[13] && Cubix[13] == Cubix[14] && Cubix[14] == Cubix[21] && ");
            sb.AppendLine("			 Cubix[21] == Cubix[22] && Cubix[22] == Cubix[23] && Cubix[23] == Cubix[30] &&   ");
            sb.AppendLine("			 Cubix[30] == Cubix[31] && Cubix[31] == Cubix[32] &&			 ");
            sb.AppendLine("			 Cubix[15] == Cubix[16] && Cubix[16] == Cubix[17] && Cubix[17] == Cubix[24] && ");
            sb.AppendLine("			 Cubix[24] == Cubix[25] && Cubix[25] == Cubix[26] && Cubix[26] == Cubix[33] &&   ");
            sb.AppendLine("			 Cubix[33] == Cubix[34] && Cubix[34] == Cubix[35] &&			 ");
            sb.AppendLine("			 Cubix[36] == Cubix[37] && Cubix[37] == Cubix[38] && Cubix[38] == Cubix[39] && ");
            sb.AppendLine("			 Cubix[39] == Cubix[40] && Cubix[40] == Cubix[41] && Cubix[41] == Cubix[42] &&   ");
            sb.AppendLine("			 Cubix[42] == Cubix[43] && Cubix[43] == Cubix[44] &&			 ");
            sb.AppendLine("			 Cubix[45] == Cubix[46] && Cubix[46] == Cubix[47] && Cubix[47] == Cubix[48] && ");
            sb.AppendLine("			 Cubix[48] == Cubix[49] && Cubix[49] == Cubix[50] && Cubix[50] == Cubix[51] &&   ");
            sb.AppendLine("			 Cubix[51] == Cubix[52] && Cubix[52] == Cubix[53];");
            sb.AppendLine("#assert Game() reaches goal;");
            sb.AppendLine("");
            sb.AppendLine("//this model models the cubix ");
            sb.AppendLine("/**************************************");
            sb.AppendLine("			0	1 	2 	 ");
            sb.AppendLine("			3	4 	5 	");
            sb.AppendLine("			6	7 	8 	");
            sb.AppendLine("9	10 	11 	12	13	14	15	16	17	");
            sb.AppendLine("18	19 	20 	21	22	23	24	25	26	");
            sb.AppendLine("27	28 	29 	30	31	32	33	34	35	");
            sb.AppendLine("			36	37 	38  ");
            sb.AppendLine("			39	40 	41");
            sb.AppendLine("			42	43 	44");
            sb.AppendLine("			45	46 	47 					51	52	53");
            sb.AppendLine("			48	49 	50					48	49	50");
            sb.AppendLine("			51	52 	53					45	46	47");
            sb.AppendLine("***************************************/");
            sb.AppendLine("#define check Cubix[4] != Cubix[19];");
            sb.AppendLine("#assert Game() |= [] check;");
      
            return sb.ToString();
        }



        //public static string LoadBakeryAlgorithm(int N, bool fair)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.AppendLine("#define SIZE " + N + ";");
        //    sb.AppendLine("var number[" + N + "];");
        //    sb.AppendLine("var choosing[" + N + "];");
        //    sb.AppendLine("var array[" + N + "];");
        //    sb.AppendLine("var temp[" + N + "];");
        //    if (fair)
        //    {
        //        sb.AppendLine("Process(i) = sl(lock_choosing.i){choosing[i] = 1; temp[i] = 0;} -> Max(i,0);");
        //    }
        //    else
        //    {
        //        sb.AppendLine("Process(i) = lock_choosing.i{choosing[i] = 1; temp[i] = 0;} -> Max(i,0);");
        //    }
        //    sb.AppendLine("Max(i,j)=");
        //    sb.AppendLine("\tif(j<SIZE)");
        //    sb.AppendLine("\t{");
        //    sb.AppendLine("\t\t(get_ticket.i.j{number[i] = temp[i] + 1;} -> unlock_choosing.i{choosing[i] = 0;} -> TryingLoop(i, 0))");
        //    sb.AppendLine("\t}");
        //    sb.AppendLine("\telse");
        //    sb.AppendLine("\t{");
        //    sb.AppendLine("\t\tif(number[j] >temp[i])");
        //    sb.AppendLine("\t\t{");
        //    sb.AppendLine("\t\t\tnumber_j_is_smaller.i.j -> Max(i, j+1)");
        //    sb.AppendLine("\t\t}");
        //    sb.AppendLine("\t\telse");
        //    sb.AppendLine("\t\t{");
        //    sb.AppendLine("\t\t\tnumber_j_is_bigger.i.j -> assign_i_to_j.i.j {temp[i] = number[j];} -> Max(i, j+1)");
        //    sb.AppendLine("\t\t}");
        //    sb.AppendLine("\t};");
        //    sb.AppendLine("TryingLoop(i, j) =");
        //    sb.AppendLine("\tif(j<SIZE)");
        //    sb.AppendLine("\t{");
        //    sb.AppendLine("\t\tgo_in_cs.i.j ->  CS(i)");
        //    sb.AppendLine("\t}");
        //    sb.AppendLine("\telse");
        //    sb.AppendLine("\t{");
        //    sb.AppendLine("\t\tj_is_small_than_size.i.j ->TryingLoopTemp(i, j)");
        //    sb.AppendLine("\t};");
        //    sb.AppendLine("TryingLoopTemp(i, j)=");
        //    sb.AppendLine("\tif(i!=j)");
        //    sb.AppendLine("\t{");
        //    sb.AppendLine("\t\ti_is_j.i.j -> TryingLoop(i, j+1)");
        //    sb.AppendLine("\t}");
        //    sb.AppendLine("\telse");
        //    sb.AppendLine("\t{");
        //    sb.AppendLine("\t\ti_is_not_j.i.j -> ");
        //    sb.AppendLine("\t\t\t([choosing[j]==0]choose_j_is_0.i.j ->");
        //    sb.AppendLine("\t\t\t\t([number[j]==0 || (number[j] > number[i]) || (number[j] == number[i] && j > i)]i_is_smaller.i.j -> TryingLoop(i, j+1))");
        //    sb.AppendLine("\t\t\t)");
        //    sb.AppendLine("\t};");
        //    sb.AppendLine("CS(i) = cs.i{array[i] = 1;} -> exit.i {number[i]=0;array[i]= 0;} -> remainder.i -> Process(i);");
        //    sb.AppendLine("//Bakery Process ");
        //    sb.AppendLine("Bakery() = |||x :{0.." + (N - 1) + "}@Process(x);");
        //    sb.AppendLine("//starvation problem.");
        //    sb.AppendLine("#assert Bakery() |= []<> cs.0;");
        //    sb.AppendLine("#assert Bakery() |= []<> (" + LoadBakeryAlgorithmUlti(N, 0) + ");");
        //    sb.AppendLine("#assert Bakery() |= " + LoadBakeryAlgorithmUlti(N, 1) + " );");
        //    sb.AppendLine("#assert Bakery() |= " + LoadBakeryAlgorithmUlti(N, 2) + ";");
        //    sb.AppendLine("//mutual exclution");
        //    sb.AppendLine("#assert Bakery() |= []!p;");
        //    sb.AppendLine("#assert Bakery() reaches p;");
        //    sb.AppendLine("#define p (" + LoadBakeryAlgorithmUlti(N, 3) + "); ");
        //    sb.AppendLine("//deadlock");
        //    sb.AppendLine("#assert Bakery() deadlockfree;");
        //    return sb.ToString();
        //}
        //private static string LoadBakeryAlgorithmUlti(int N, int inCase)
        //{
        //    //cs.1 || cs.0
        //    if (inCase == 0)
        //    {
        //        string result = "cs.0";
        //        for (int i = 1; i <= N - 1; i++)
        //        {
        //            result += " ||cs." + i;
        //        }
        //        return result;
        //    }
        //    //([]<> lock_choosing.0 && []<> lock_choosing.1) -> []<>(cs.1 || cs.0)
        //    else if (inCase == 1)
        //    {
        //        string result = "([]<> lock_choosing.0";
        //        for (int i = 1; i <= N - 1; i++)
        //        {
        //            result += "&& []<> lock_choosing." + i;
        //        }
        //        result += ") -> []<>("+LoadBakeryAlgorithmUlti(N,0);
        //        return result;
        //    }
        //    //([]<> lock_choosing.0 && []<> lock_choosing.1) -> []<>cs.1
        //    else if (inCase == 2)
        //    {
        //        string result = "([]<> lock_choosing.0";
        //        for (int i = 1; i <= N - 1; i++)
        //        {
        //            result += "&& []<> lock_choosing." + i;
        //        }
        //        result += ")-> []<>cs." + (N - 1);
        //        return result;
        //    }
        //    //array[0]==1 && array[1]==1
        //    else if (inCase == 3)
        //    {
        //        string result = "array[0]==1";
        //        for (int i = 1; i <= N - 1; i++)
        //        {
        //            result += " && array[" + i + "]==1";
        //        }
        //        return result;
        //    }
        //    else
        //    {
        //        return string.Empty;
        //    }
        //}

        public static string LoadDijkstra(int N)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("#define IDLE 0;");
            sb.AppendLine("#define GET_TURN 1;");
            sb.AppendLine("#define CHECK 2;");
            sb.AppendLine("#define SIZE " + N + ";");
            sb.AppendLine("var status[SIZE];");
            sb.AppendLine("var array[SIZE];");
            sb.AppendLine("/*=======================================================");
            sb.AppendLine("Dijkstra's Mutual Exclutiosn Algo");
            sb.AppendLine("Process i");
            sb.AppendLine("===================trying section========================");
            sb.AppendLine("repeat");
            sb.AppendLine("\tstatus[i] := CHECK");
            sb.AppendLine("\tfor j != i do");
            sb.AppendLine("\t\tif status[j] = CHECK");
            sb.AppendLine("\t\t\tstatus[i] = GET_TURN");
            sb.AppendLine("until status[i] = check");
            sb.AppendLine("===================critial section========================");
            sb.AppendLine("\t{CS}");
            sb.AppendLine("===================exiting section========================");
            sb.AppendLine("\tstatus[i] = IDLE");
            sb.AppendLine("===================remainder section======================");
            sb.AppendLine("{RS}");
            sb.AppendLine("===========================end============================*/");
            sb.AppendLine("Process(i) = set_status_checked.i{status[i] = CHECK;} -> TryingLoop2(i, 0);");
            sb.AppendLine("TryingLoop2(i, j) = ");
            sb.AppendLine("\tifa(j<SIZE)");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tif(status[i] == CHECK)");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tj_is_big_than_size.i.j  -> retry.i.j -> Process(i)");
            sb.AppendLine("\t}");
            sb.AppendLine("\t\telse");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tj_is_big_than_size.i.j  -> go_in_cs.i.j ->  CS(i) ");
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");
            sb.AppendLine("\telse");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tifa(i != j)");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tj_is_small_than_size.i.j ->i_is_not_j.i.j -> TryingLoop2(i, j+1)");
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t\telse");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tif(status[j] == CHECK)");
            sb.AppendLine("\t\t\t{");
            sb.AppendLine("\t\t\t\tj_is_small_than_size.i.j ->i_is_not_j.i.j ->status_j_is_not_check.i.j -> TryingLoop2(i, j+1)");
            sb.AppendLine("\t\t\t}");
            sb.AppendLine("\t\t\telse");
            sb.AppendLine("\t\t\t{");
            sb.AppendLine("\t\t\t\tj_is_small_than_size.i.j ->i_is_not_j.i.j ->status_j_is_check.i.j -> give_up.i{status[i]=GET_TURN;} -> TryingLoop2(i, j+1) ");
            sb.AppendLine("\t\t\t}");
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t};");
            sb.AppendLine("CS(i) = cs.i{array[i] = 1;} -> exit.i {status[i]=IDLE; array[i]= 0;} -> remainder.i -> Process(i);");
            sb.AppendLine("Dijkstra() = |||x :{0.." + (N - 1) + "}@Process(x);");
            sb.AppendLine("//the following case is not true");
            sb.AppendLine("//there is a case where P0 and P1 always set their status to check and then give up in the loop");
            sb.AppendLine("#assert Dijkstra() |= (" + LoadDijkstraNoLoopUlti(N, 0) + ") -> []<>(" + LoadDijkstraNoLoopUlti(N, 1) + ");");
            sb.AppendLine("#assert Dijkstra() |= []<>cs.0;");
            sb.AppendLine("#assert Dijkstra() |= (" + LoadDijkstraNoLoopUlti(N, 0) + ") -> []<>(cs.0);");
            sb.AppendLine("#assert Dijkstra() |= (" + LoadDijkstraNoLoopUlti(N, 0) + ") -> []<>(cs.1);");
            sb.AppendLine("//mutual exclution");
            sb.AppendLine("#assert Dijkstra() |= []!p;");
            sb.AppendLine("#assert Dijkstra() reaches p;");
            sb.AppendLine("#define p (" + LoadDijkstraNoLoopUlti(N, 2) + "); ");
            return sb.ToString();

        }
        private static string LoadDijkstraNoLoopUlti(int N, int inCase)
        {
            //[]<> set_status_checked.0 && []<> set_status_checked.1
            if (inCase == 0)
            {
                string result = "[]<> set_status_checked.0";
                for (int i = 1; i <= N - 1; i++)
                {
                    result += "&& []<> set_status_checked." + i;
                }
                return result;
            }
            //cs.1 || cs.0
            else if (inCase == 1)
            {
                string result = "cs.0";
                for (int i = 1; i <= N - 1; i++)
                {
                    result += " ||cs." + i;
                }
                return result;
            }
            //array[0]==1 && array[1]==1
            else if (inCase == 2)
            {
                string result = "array[0]==1";
                for (int i = 1; i <= N - 1; i++)
                {
                    result += " && array[" + i + "]==1";
                }
                return result;
            }
            else
            {
                return string.Empty;
            }
        }

        public static string LoadDijkstraFair(int N)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("#define IDLE 0;");
            sb.AppendLine("#define GET_TURN 1;");
            sb.AppendLine("#define CHECK " + N + ";");
            sb.AppendLine("#define SIZE " + N + ";");
            sb.AppendLine("");
            sb.AppendLine("var status[" + N + "];");
            sb.AppendLine("var turn = 0;");
            sb.AppendLine("var array[" + N + "];");
            sb.AppendLine("");
            sb.AppendLine("/*=======================================================");
            sb.AppendLine("Dijkstra's Mutual Exclutiosn Algo");
            sb.AppendLine("Process i");
            sb.AppendLine("===================trying section========================");
            sb.AppendLine("\tstatus[i] = GET_TRUN;");
            sb.AppendLine("  repeat");
            sb.AppendLine("\t\twhile turn != i do");
            sb.AppendLine("\t\t\tif status[turn] = IDLE");
            sb.AppendLine("\t\t\t\tturn = i");
            sb.AppendLine("\t\tstatus[i] := CHECK");
            sb.AppendLine("\t\tfor j != i do");
            sb.AppendLine("\t\t\tif status[j] = CHECK ");
            sb.AppendLine("\t\t\t\tstatus[i] = GET_TURN");
            sb.AppendLine("\tuntil status[i] = check");
            sb.AppendLine("===================critial section========================");
            sb.AppendLine("\t{CS}");
            sb.AppendLine("===================exiting section========================");
            sb.AppendLine("\tstatus[i] = IDLE\t\t\t\t\t");
            sb.AppendLine("===================remainder section======================");
            sb.AppendLine("\t{RS}");
            sb.AppendLine("===========================end============================*/");
            sb.AppendLine("");
            sb.AppendLine("Process(i) = sl(entry.i){status[i] = GET_TURN;} -> TryingLoop1(i);");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("TryingLoop1(i) = ");
            sb.AppendLine("\tif(turn != i)");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tsl(turn_is_i.i) -> sl(set_status_checked.i){status[i] = CHECK;} -> TryingLoop2(i, 0)");
            sb.AppendLine("\t}");
            sb.AppendLine("\telse");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tsl(turn_is_not_i.i) ->TryingLoop1Temp(i)");
            sb.AppendLine("\t\t");
            sb.AppendLine("\t};");
            sb.AppendLine("\t");
            sb.AppendLine("TryingLoop1Temp(i)=");
            sb.AppendLine("\t\tif(status[turn] == IDLE)");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tsl(turn_not_idle.i) -> TryingLoop1(i)");
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t\telse");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tsl(turn_idle.i) -> sl(assign.i){turn=i;} -> TryingLoop1(i)");
            sb.AppendLine("\t\t};");
            sb.AppendLine("\t\t");
            sb.AppendLine("//----------------------------");
            sb.AppendLine("TryingLoop2(i, j) = ");
            sb.AppendLine("\tif(j<SIZE)");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\t sl(j_is_big_than_size.i.j) ->TryingLoop2Temp1(i, j)");
            sb.AppendLine("\t}");
            sb.AppendLine("\telse");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tsl(j_is_small_than_size.i.j) ->TryingLoop2Temp2(i, j)");
            sb.AppendLine("\t};");
            sb.AppendLine("TryingLoop2Temp1(i, j)=");
            sb.AppendLine("\tif(status[i] == CHECK)");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tsl(retry.i.j) -> TryingLoop1(i)");
            sb.AppendLine("\t}");
            sb.AppendLine("\telse");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tsl(go_in_cs.i.j) ->  CS(i)");
            sb.AppendLine("\t};");
            sb.AppendLine("\t");
            sb.AppendLine("TryingLoop2Temp2(i, j)=");
            sb.AppendLine("\tif(i != j)");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tsl(i_is_j.i.j) -> TryingLoop2(i, j+1)");
            sb.AppendLine("\t}");
            sb.AppendLine("\telse");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tsl(i_is_not_j.i.j) ->TryingLoop2Temp3(i, j)");
            sb.AppendLine("\t};");
            sb.AppendLine("\t");
            sb.AppendLine("TryingLoop2Temp3(i, j)=");
            sb.AppendLine("\tif(status[j] == CHECK)");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tsl(status_j_is_not_check.i.j) -> TryingLoop2(i, j+1)");
            sb.AppendLine("\t}");
            sb.AppendLine("\telse");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tsl(status_j_is_check.i.j) -> sl(give_up.i){status[i]=GET_TURN;} -> TryingLoop2(i, j+1)");
            sb.AppendLine("\t};");
            sb.AppendLine("\t");
            sb.AppendLine("//---------------------------");
            sb.AppendLine("");
            sb.AppendLine("CS(i) = sl(cs.i){array[i] = 1;} -> sl(exit.i) {status[i]=IDLE; array[i]= 0;} -> sl(remainder.i) -> Process(i);");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("Dijkstra() = |||x :{0.." + (N - 1) + "}@Process(x);");
            sb.AppendLine("");
            sb.AppendLine("//livelock: (also called deadlock in share memory book)");
            sb.AppendLine("//in every admissible execution, if there is some process in the trying section,");
            sb.AppendLine("//then some process (may not the same one) will entry the critial section eventually.");
            sb.AppendLine("");
            sb.AppendLine("//in an admissible execution, each process must have infinite step or ends in the remainder section.");
            sb.AppendLine("");
            sb.AppendLine("//is the following correct?");
            sb.AppendLine("//the admissible means all the processes have infinite steps.");
            sb.AppendLine("//can mark all trying section events to be strong live? ");
            sb.AppendLine("//#assert Dijkstra() |= []<> (LoadDijkstraNoLoopUlti(N,1));");
            sb.AppendLine("#assert Dijkstra() |= (" + LoadDijkstraNoLoopUlti(N, 0) + ") -> []<>(" + LoadDijkstraNoLoopUlti(N, 1) + ");");
            sb.AppendLine("#assert Dijkstra() |= []<>cs.0;");
            sb.AppendLine("");
            sb.AppendLine("//Lockout (starvation problem.)");
            sb.AppendLine("//in every admissible execution, if there is some process in the trying section,");
            sb.AppendLine("//then the same process will entry the critial section eventually.");
            sb.AppendLine("#assert Dijkstra() |= (" + LoadDijkstraNoLoopUlti(N, 0) + ") -> []<>(cs.0);");
            sb.AppendLine("#assert Dijkstra() |= (" + LoadDijkstraNoLoopUlti(N, 0) + ") -> []<>(cs.1);");
            sb.AppendLine("");
            sb.AppendLine("//#assert Dijkstra() |= ([]<> entry.0) -> []<>(cs.0) ;");
            sb.AppendLine("");
            sb.AppendLine("//mutual exclution");
            sb.AppendLine("#assert Dijkstra() |= []!p;");
            sb.AppendLine("#assert Dijkstra() reaches p;");
            sb.AppendLine("#define p (" + LoadDijkstraNoLoopUlti(N, 2) + "); ");
            sb.AppendLine("");
            sb.AppendLine("//deadlock");
            sb.AppendLine("#assert Dijkstra() deadlockfree;");
            sb.AppendLine("");
            sb.AppendLine("//when process 0 is in critical section, the turn must 0");
            sb.AppendLine("#assert Dijkstra() |= []q;");
            sb.AppendLine("#assert Dijkstra() reaches q;");
            sb.AppendLine("#define q (" + LoadDijkstraNoLoopUlti(N, 2) + "); ");

            return sb.ToString();
        }


        public static string LoadSlidingGame(int Size)
        {
            if (Size == 3)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("//@@Sliding Game@@");
                sb.AppendLine("//The following models the class sliding game.");
                sb.AppendLine("//The following is the board setting.");
                sb.AppendLine("// 0 \t1 \t2 \t");
                sb.AppendLine("// 3    4 \t5 \t");
                sb.AppendLine("// 6 \t7   8 ");
                sb.AppendLine("");
                sb.AppendLine("var board = [3,5,6,0,2,7,8,4,1];");
                sb.AppendLine("hvar emptypos = 3; //emptypos is a secondary variable, no need to put it in the state space");
                sb.AppendLine("");
                sb.AppendLine("Game() = Left() [] Right() [] Up() [] Down();");
                sb.AppendLine("");
                sb.AppendLine("Left() = [emptypos!=2&&emptypos!=5&&emptypos!=8]goleft{board[emptypos]=board[emptypos+1];board[emptypos+1]=0;emptypos=emptypos+1;} -> Game();");
                sb.AppendLine("Right() = [emptypos!=0&&emptypos!=3&&emptypos!=6]goright{board[emptypos]=board[emptypos-1];board[emptypos-1]=0;emptypos=emptypos-1;} -> Game();");
                sb.AppendLine("Up() = [emptypos!=6&&emptypos!=7&&emptypos!=8]goup{board[emptypos]=board[emptypos+3];board[emptypos+3]=0;emptypos=emptypos+3;} -> Game();");
                sb.AppendLine("Down() = [emptypos!=0&&emptypos!=1&&emptypos!=2]godown{board[emptypos]=board[emptypos-3];board[emptypos-3]=0;emptypos=emptypos-3;} -> Game();");
                sb.AppendLine("");
                sb.AppendLine("#assert Game() reaches goal;");
                sb.AppendLine("");
                sb.AppendLine("#define goal board[0] == 1 && board[1] == 2 && board[2] == 3 && board[3] == 4 && board[4] == 5 && board[5] == 6 && board[6] == 7 &&  board[7] == 8 && board[8] == 0;");
                return sb.ToString();
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("//@@Sliding Game@@");
                sb.AppendLine("//The following models the class sliding game.");
                sb.AppendLine("//The following is the board setting.");
                sb.AppendLine("// 0 \t1 \t2 \t3");
                sb.AppendLine("// 4 \t5 \t6 \t7");
                sb.AppendLine("// 8 \t9 \t10 \t11");
                sb.AppendLine("// 12 \t13 \t14 \t15");
                sb.AppendLine("");
                sb.AppendLine("var board = [3,5,6,0,2,7,8,4,1,10,15,12,9,13,11,14];");
                sb.AppendLine("hvar emptypos = 3; //emptypos is a secondary variable, no need to put it in the state space");
                sb.AppendLine("");
                sb.AppendLine("Game() = Left() [] Right() [] Up() [] Down();");
                sb.AppendLine("");
                sb.AppendLine("Left() = [emptypos!=3&&emptypos!=7&&emptypos!=11&&emptypos!=15]goleft{board[emptypos]=board[emptypos+1];board[emptypos+1]=0;emptypos=emptypos+1;} -> Game();");
                sb.AppendLine("Right() = [emptypos!=0&&emptypos!=4&&emptypos!=8&&emptypos!=12]goright{board[emptypos]=board[emptypos-1];board[emptypos-1]=0;emptypos=emptypos-1;} -> Game();");
                sb.AppendLine("Up() = [emptypos!=12&&emptypos!=13&&emptypos!=14&&emptypos!=15]goup{board[emptypos]=board[emptypos+4];board[emptypos+4]=0;emptypos=emptypos+4;} -> Game();");
                sb.AppendLine("Down() = [emptypos!=0&&emptypos!=1&&emptypos!=2&&emptypos!=3]godown{board[emptypos]=board[emptypos-4];board[emptypos-4]=0;emptypos=emptypos-4;} -> Game();");
                sb.AppendLine("");
                sb.AppendLine("#assert Game() reaches goal;");
                sb.AppendLine("");
                sb.AppendLine("#define goal board[0] == 1 && board[1] == 2 && board[2] == 3 && board[3] == 4 && board[4] == 5 && board[5] == 6 && board[6] == 7 &&  board[7] == 8 && board[8] == 9 && board[9] == 10 && board[10] == 11 && board[11] == 12 && board[12] == 13 && board[13] == 14 && board[14] == 15 && board[15] == 0;");
                return sb.ToString();
            }
        }

        public static string LoadLightOff()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/*The following models the game LightOff (available for iPhone).*/");
            sb.AppendLine("/*The following is the drawing of the board settings.*/");
            sb.AppendLine("/////////////////////////// 0  1  2  3  4/////////////////////////");
            sb.AppendLine("/////////////////////////// 5  6  7  8  9/////////////////////////");
            sb.AppendLine("///////////////////////////10 11 12 13 14/////////////////////////");
            sb.AppendLine("///////////////////////////15 16 17 18 19/////////////////////////");
            sb.AppendLine("///////////////////////////20 21 22 23 24/////////////////////////");
            sb.AppendLine("");
            sb.AppendLine("//The following is the setting of Stage 9.");
            sb.AppendLine("var a00 = 0; var a01 = 1; var a02 = 1; var a03 = 0; var a04 = 0;");
            sb.AppendLine("var a05 = 0; var a06 = 1; var a07 = 1; var a08 = 1; var a09 = 0;");
            sb.AppendLine("var a10 = 0; var a11 = 1; var a12 = 0; var a13 = 1; var a14 = 0; ");
            sb.AppendLine("var a15 = 0; var a16 = 1; var a17 = 1; var a18 = 1; var a19 = 0;");
            sb.AppendLine("var a20 = 0; var a21 = 0; var a22 = 1; var a23 = 1; var a24 = 0;");
            sb.AppendLine("");
            sb.AppendLine("LightsOff() = click0{a00=1-a00;a01=1-a01;a05=1-a05;} -> LightsOff()");
            sb.AppendLine("\t\t\t[] click1{a00=1-a00;a01=1-a01;a02=1-a02;a06=1-a06;} -> LightsOff() ");
            sb.AppendLine("\t\t\t[] click2{a01=1-a01;a02=1-a02;a03=1-a03;a07=1-a07;} -> LightsOff()");
            sb.AppendLine("\t\t\t[] click3{a02=1-a02;a03=1-a03;a04=1-a04;a08=1-a08;} -> LightsOff()");
            sb.AppendLine("\t\t\t[] click4{a03=1-a03;a04=1-a04;a09=1-a09;} -> LightsOff()");
            sb.AppendLine("\t\t\t[] click5{a00=1-a00;a05=1-a05;a06=1-a06;a10=1-a10;} -> LightsOff()");
            sb.AppendLine("\t\t\t[] click6{a01=1-a01;a05=1-a05;a06=1-a06;a07=1-a07;a11=1-a11;} -> LightsOff()");
            sb.AppendLine("\t\t\t[] click7{a02=1-a02;a06=1-a06;a07=1-a07;a08=1-a08;a12=1-a12;} -> LightsOff()");
            sb.AppendLine("\t\t\t[] click8{a03=1-a03;a07=1-a07;a08=1-a08;a09=1-a09;a13=1-a13;} -> LightsOff()");
            sb.AppendLine("\t\t\t[] click9{a04=1-a04;a08=1-a08;a09=1-a09;a14=1-a14;} -> LightsOff()");
            sb.AppendLine("\t\t\t[] click10{a05=1-a05;a10=1-a10;a11=1-a11;a15=1-a15;} -> LightsOff()");
            sb.AppendLine("\t\t\t[] click11{a06=1-a06;a10=1-a10;a11=1-a11;a12=1-a12;a16=1-a16;} -> LightsOff()");
            sb.AppendLine("\t\t\t[] click12{a07=1-a07;a11=1-a11;a12=1-a12;a13=1-a13;a17=1-a17;} -> LightsOff()");
            sb.AppendLine("\t\t\t[] click13{a08=1-a08;a12=1-a12;a13=1-a13;a14=1-a14;a18=1-a18;} -> LightsOff()");
            sb.AppendLine("\t\t\t[] click14{a09=1-a09;a13=1-a13;a14=1-a14;a19=1-a19;} ->  LightsOff()");
            sb.AppendLine("\t\t\t[] click15{a10=1-a10;a15=1-a15;a16=1-a16;a20=1-a20;} -> LightsOff()");
            sb.AppendLine("\t\t\t[] click16{a11=1-a11;a15=1-a15;a16=1-a16;a17=1-a17;a21=1-a21;} -> LightsOff()");
            sb.AppendLine("\t\t\t[] click17{a12=1-a12;a16=1-a16;a17=1-a17;a18=1-a18;a22=1-a22;} -> LightsOff()");
            sb.AppendLine("\t\t\t[] click18{a13=1-a13;a17=1-a17;a18=1-a18;a19=1-a19;a23=1-a23;} -> LightsOff()");
            sb.AppendLine("\t\t\t[] click19{a14=1-a14;a18=1-a18;a19=1-a19;a24=1-a24;} -> LightsOff()");
            sb.AppendLine("\t\t\t[] click20{a15=1-a15;a20=1-a20;a21=1-a21;} -> LightsOff()");
            sb.AppendLine("\t\t\t[] click21{a16=1-a16;a20=1-a20;a21=1-a21;a22=1-a22;} -> LightsOff()");
            sb.AppendLine("\t\t\t[] click22{a17=1-a17;a21=1-a21;a22=1-a22;a23=1-a23;} -> LightsOff()");
            sb.AppendLine("\t\t\t[] click23{a18=1-a18;a22=1-a22;a23=1-a23;a24=1-a24;} -> LightsOff()");
            sb.AppendLine("\t\t\t[] click24{a19=1-a19;a23=1-a23;a24=1-a24;} -> LightsOff();");
            sb.AppendLine("\t\t\t");
            sb.AppendLine("//============================LTL Definitions============================");
            sb.AppendLine("#define goal (a00+a01+a02+a03+a04+a05+a06+a07+a08+a09+a10+a11+a12+a13+a14+a15+a16+a17+a18+a19+a20+a21+a22+a23+a24==0);");
            sb.AppendLine("");
            sb.AppendLine("#assert LightsOff() reaches goal;");
            return sb.ToString();


        }


        public static string LoadKnightTour(int N)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("#define N " + N + ";");
            sb.AppendLine("");
            sb.AppendLine("//the board is a N*N matrix");
            sb.AppendLine("var board[N*N];");
            sb.AppendLine("var steps = 0;");
            sb.AppendLine("");
            sb.AppendLine("//there are 8 ways of jumping");
            sb.AppendLine("// 0 1");
            sb.AppendLine("//2   3");
            sb.AppendLine("//  k");
            sb.AppendLine("//4   5");
            sb.AppendLine("// 6 7 ");
            sb.AppendLine("Knight(i, j) = [i-2 >= 0 && j-1 >=0] Move0(i,j)	[]");
            sb.AppendLine("       	       [i-2 >= 0 && j+1 < N] Move1(i,j)	[]");
            sb.AppendLine("			   [i-1 >= 0 && j-2 >=0] Move2(i,j) []");
            sb.AppendLine("			   [i-1 >= 0 && j+2 < N] Move3(i,j) []");
            sb.AppendLine("			   [i+1 < N && j-2 >=0] Move4(i,j)  []");
            sb.AppendLine("			   [i+1 < N && j+2 < N] Move5(i,j)	[]");
            sb.AppendLine("			   [i+2 < N && j-1 >=0] Move6(i,j)	[]");
            sb.AppendLine("			   [i+2 < N && j+1 < N] Move7(i,j);			   ");
            sb.AppendLine("");
            sb.AppendLine("//each jump will update the board and counter");
            sb.AppendLine("Move0(i, j) = [board[(i-2)*N+j-1]==0]jump0{board[(i-2)*N+j-1]=1;steps++} -> Knight(i-2, j-1); ");
            sb.AppendLine("Move1(i, j) = [board[(i-2)*N+j+1]==0]jump1{board[(i-2)*N+j+1]=1;steps++} -> Knight(i-2, j+1);");
            sb.AppendLine("Move2(i, j) = [board[(i-1)*N+j-2]==0]jump2{board[(i-1)*N+j-2]=1;steps++} -> Knight(i-1, j-2);");
            sb.AppendLine("Move3(i, j) = [board[(i-1)*N+j+2]==0]jump3{board[(i-1)*N+j+2]=1;steps++} -> Knight(i-1, j+2);");
            sb.AppendLine("Move4(i, j) = [board[(i+1)*N+j-2]==0]jump4{board[(i+1)*N+j-2]=1;steps++} -> Knight(i+1, j-2);");
            sb.AppendLine("Move5(i, j) = [board[(i+1)*N+j+2]==0]jump5{board[(i+1)*N+j+2]=1;steps++} -> Knight(i+1, j+2);");
            sb.AppendLine("Move6(i, j) = [board[(i+2)*N+j-1]==0]jump6{board[(i+2)*N+j-1]=1;steps++} -> Knight(i+2, j-1);");
            sb.AppendLine("Move7(i, j) = [board[(i+2)*N+j+1]==0]jump7{board[(i+2)*N+j+1]=1;steps++} -> Knight(i+2, j+1);");
            sb.AppendLine("");
            sb.AppendLine("Game(i, j) = start{board[i*N+j] = 1} -> Knight(i, j);");

            sb.AppendLine("GameInstance = Game(0,0);");
            sb.Append("#define goal (");

            for (int i = 0; i < N*N -1; i++)
            {
                sb.Append("board["+i+"] == 1 && ");
            }
            sb.AppendLine("board[" + (N*N - 1) + "] == 1);"); 

            sb.AppendLine("#define altergoal steps == N*N-1;");
            sb.AppendLine("");
            sb.AppendLine("#assert GameInstance reaches goal;");
            sb.AppendLine("#assert GameInstance reaches altergoal;");
            return sb.ToString();
        }

        public static string LoadPegGame(int N)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("#define X -1;");
            sb.AppendLine("#define P 1;");
            sb.AppendLine("#define E 2;");
            sb.AppendLine("#define S 1; //sticky has the same value as P now.");
            sb.AppendLine("");

            sb.AppendLine("//===== Board " + N +" =========");
            if(N == 1)
            {
                sb.AppendLine("#define initEmptyX 3;");
                sb.AppendLine("#define initEmptyY 3;");
                sb.AppendLine("#define W 7;");
                sb.AppendLine("#define H 7;");
                sb.AppendLine("var board[H][W] = ");
                sb.AppendLine("		  [X,X,P,P,P,X,X,");
                sb.AppendLine("           X,X,P,P,P,X,X,");
                sb.AppendLine("           S,S,P,P,P,P,P,");
                sb.AppendLine("           P,P,P,E,P,P,P,");
                sb.AppendLine("           S,S,P,P,P,P,P,");
                sb.AppendLine("           X,X,S,P,S,X,X,");
                sb.AppendLine("           X,X,S,P,S,X,X];");
                sb.AppendLine("    ");
                sb.AppendLine("var pegsCounter = 32;");    
            }
            else if (N == 2)
            {
                sb.AppendLine("#define initEmptyX 2;");
                sb.AppendLine("#define initEmptyY 3;");
                sb.AppendLine("#define W 7;");
                sb.AppendLine("#define H 6;");
                sb.AppendLine("var board[H][W] = ");
                sb.AppendLine("		  [X,X,P,P,P,X,X,");
                sb.AppendLine("           S,S,P,P,P,P,P,");
                sb.AppendLine("           P,P,P,E,P,P,P,");
                sb.AppendLine("           S,S,P,P,P,P,P,");
                sb.AppendLine("           X,X,S,P,S,X,X,");
                sb.AppendLine("           X,X,S,P,S,X,X];");
                sb.AppendLine("    ");
                sb.AppendLine("var pegsCounter = 29;");
            }
            else if (N == 3)
            {
                sb.AppendLine("#define initEmptyX 2;");
                sb.AppendLine("#define initEmptyY 3;");
                sb.AppendLine("#define W 6;");
                sb.AppendLine("#define H 6;");
                sb.AppendLine("var board[H][W] = ");
                sb.AppendLine("	         [X,X,P,P,P,X,");
                sb.AppendLine("           S,S,P,P,P,P,");
                sb.AppendLine("           P,P,P,E,P,P,");
                sb.AppendLine("           S,S,P,P,P,P,");
                sb.AppendLine("           X,X,S,P,S,X,");
                sb.AppendLine("           X,X,S,P,S,X];");
                sb.AppendLine("    ");
                sb.AppendLine("var pegsCounter = 26;");
            }
            else if (N == 4)
            {
                sb.AppendLine("#define initEmptyX 2;");
                sb.AppendLine("#define initEmptyY 2;");
                sb.AppendLine("#define W 5;");
                sb.AppendLine("#define H 6;");
                sb.AppendLine("var board[H][W] = ");
                sb.AppendLine("	         [X,P,P,P,X,");
                sb.AppendLine("           S,P,P,P,P,");
                sb.AppendLine("           P,P,E,P,P,");
                sb.AppendLine("           S,P,P,P,P,");
                sb.AppendLine("           X,S,P,S,X,");
                sb.AppendLine("           X,S,P,S,X];");
                sb.AppendLine("    ");
                sb.AppendLine("var pegsCounter = 23;");
            }
            else if(N == 5)
            {
                sb.AppendLine("#define initEmptyX 1;");
                sb.AppendLine("#define initEmptyY 2;");
                sb.AppendLine("#define W 5;");
                sb.AppendLine("#define H 5;");
                sb.AppendLine("var board[H][W] = ");
                sb.AppendLine("	         [S,P,P,P,P,");
                sb.AppendLine("           P,P,E,P,P,");
                sb.AppendLine("           S,P,P,P,P,");
                sb.AppendLine("           X,S,P,S,X,");
                sb.AppendLine("           X,S,P,S,X];");
                sb.AppendLine("    ");
                sb.AppendLine("var pegsCounter = 20;");
            }
            else if (N == 6)
            {
                sb.AppendLine("#define initEmptyX 4;");
                sb.AppendLine("#define initEmptyY 3;");
                sb.AppendLine("#define W 7;");
                sb.AppendLine("#define H 8;");
                sb.AppendLine("var board[H][W] = ");
                sb.AppendLine("	         [X,X,P,P,P,X,X,");
                sb.AppendLine("           X,X,P,P,P,X,X,");
                sb.AppendLine("           X,X,P,P,P,X,X,");
                sb.AppendLine("           S,S,P,P,P,P,P,");
                sb.AppendLine("           P,P,P,E,P,P,P,");
                sb.AppendLine("           S,S,P,P,P,P,P,");
                sb.AppendLine("           X,X,S,P,S,X,X,");
                sb.AppendLine("           X,X,S,P,S,X,X];");
                sb.AppendLine("    ");
                sb.AppendLine("var pegsCounter = 48;");
            }
            else if (N == 7)
            {
                sb.AppendLine("#define initEmptyX 4;");
                sb.AppendLine("#define initEmptyY 3;");
                sb.AppendLine("#define W 7;");
                sb.AppendLine("#define H 8;");
                sb.AppendLine("var board[H][W] = ");
                sb.AppendLine("	         [X,X,P,P,P,P,X,");
                sb.AppendLine("           X,X,P,P,P,P,X,");
                sb.AppendLine("           X,X,P,P,P,P,X,");
                sb.AppendLine("           S,S,P,P,P,P,P,");
                sb.AppendLine("           S,S,P,E,P,P,P,");
                sb.AppendLine("           S,S,P,P,P,P,P,");
                sb.AppendLine("           X,X,S,S,S,X,X,");
                sb.AppendLine("           X,X,S,S,S,X,X];");
                sb.AppendLine("    ");
                sb.AppendLine("var pegsCounter = 38;");
            }
            else if (N == 8)
            {
                sb.AppendLine("#define initEmptyX 4;");
                sb.AppendLine("#define initEmptyY 3;");
                sb.AppendLine("#define W 7;");
                sb.AppendLine("#define H 8;");
                sb.AppendLine("var board[H][W] = ");
                sb.AppendLine("	         [X,P,P,P,P,P,X,");
                sb.AppendLine("           X,P,P,P,P,P,X,");
                sb.AppendLine("           X,P,P,P,P,P,X,");
                sb.AppendLine("           S,S,P,P,P,P,P,");
                sb.AppendLine("           S,S,P,E,P,P,P,");
                sb.AppendLine("           S,S,P,P,P,S,S,");
                sb.AppendLine("           X,X,S,S,S,X,X,");
                sb.AppendLine("           X,X,S,S,S,X,X];");
                sb.AppendLine("    ");
                sb.AppendLine("var pegsCounter = 41;");
            }
            else if (N == 9)
            {
                sb.AppendLine("#define initEmptyX 4;");
                sb.AppendLine("#define initEmptyY 3;");
                sb.AppendLine("#define W 7;");
                sb.AppendLine("#define H 8;");
                sb.AppendLine("var board[H][W] = ");
                sb.AppendLine("	         [P,P,P,P,P,P,P,");
                sb.AppendLine("           P,P,P,P,P,P,P,");
                sb.AppendLine("           P,P,P,P,P,P,P,");
                sb.AppendLine("           S,S,P,P,P,P,P,");
                sb.AppendLine("           S,S,P,E,P,P,S,");
                sb.AppendLine("           S,S,S,P,S,S,S,");
                sb.AppendLine("           X,X,S,S,S,X,X,");
                sb.AppendLine("           X,X,S,S,S,X,X];");
                sb.AppendLine("    ");
                sb.AppendLine("var pegsCounter = 47;");

            }
            sb.AppendLine("");
            sb.AppendLine("//four different ways of jumping");
            sb.AppendLine("Up(i, j) = [i-2>=0]([board[i-2][j]==E && board[i-1][j]== P]up{board[i-2][j] = P; board[i-1][j] = E; board[i][j] = E; pegsCounter--;} -> Game()); ");
            sb.AppendLine("Left(i, j) = [j-2>=0]([board[i][j-2]==E && board[i][j-1]== P]left{board[i][j-2] = P; board[i][j-1] = E; board[i][j] = E; pegsCounter--;} -> Game()); ");
            sb.AppendLine("Down(i, j) = [i+2<H]([board[i+2][j] != X && board[i+2][j]==E && board[i+1][j]== P]down{board[i+2][j] = P; board[i+1][j] = E; board[i][j] = E; pegsCounter--;} -> Game());  ");
            sb.AppendLine("Right(i, j) = [j+2<W]([board[i][j+2] != X && board[i][j+2]==E && board[i][j+1]== P]right{board[i][j+2] = P; board[i][j+1] = E; board[i][j] = E; pegsCounter--;} -> Game()); ");
            sb.AppendLine("");
            sb.AppendLine("//if there is a peg in the cell, it makes four diffferent moves");
            sb.AppendLine("Peg(i,j) = [board[i][j]==P](Up(i,j) [] Left(i,j) [] Down(i,j) [] Right(i,j));");
            sb.AppendLine("Game() = []i:{0..H-1}@[]j:{0..W-1}@ Peg(i,j);");

            sb.AppendLine("");
            sb.AppendLine("#define goal pegsCounter == 1 && board[initEmptyX][initEmptyY] == P;");
            sb.AppendLine("#assert Game() reaches goal;");
            sb.AppendLine(" ");
            return sb.ToString();
        }

        public static string LoadShunting(bool originalModel)
        {
            StringBuilder sb = new StringBuilder();

            if (!originalModel)
            {
                sb.AppendLine("#import \"PAT.Lib.Board\";");
                sb.AppendLine("var<Board> board;");
                sb.AppendLine("");
                sb.AppendLine("Game() = [!board.GameOver()](");
                sb.AppendLine("                       moveup{board.MoveUp()} -> Game()");
                sb.AppendLine("                       []");
                sb.AppendLine("                       pushup{board.PushUp()} -> Game()");
                sb.AppendLine("                       []");
                sb.AppendLine("                       movedown{board.MoveDown()} -> Game()");
                sb.AppendLine("                       []");
                sb.AppendLine("                       pushdown{board.PushDown()} -> Game()");
                sb.AppendLine("                       []");
                sb.AppendLine("                       moveleft{board.MoveLeft()} -> Game()");
                sb.AppendLine("                       []");
                sb.AppendLine("                       pushleft{board.PushLeft()} -> Game()");
                sb.AppendLine("                       []");
                sb.AppendLine("                       moveright{board.MoveRight()} -> Game()");
                sb.AppendLine("                       []");
                sb.AppendLine("                       pushright{board.PushRight()} -> Game()");
                sb.AppendLine("               );");
                sb.AppendLine("");
                sb.AppendLine("#assert Game() deadlockfree;");
            }
            else
            {
                sb.AppendLine("//@@Shunting Game@@");
                sb.AppendLine(
                    "//The following are constants of the shunting game; They are defined only for readability.");
                sb.AppendLine("#define M 7;");
                sb.AppendLine("#define N 6;");
                sb.AppendLine("#define o -1;");
                sb.AppendLine("#define a 1;");
                sb.AppendLine("#define w 0;");
                sb.AppendLine("");
                sb.AppendLine("//The following are variables of the system, which constitute the state space.");
                sb.AppendLine("//The initial valuation of the variables corresponds to the initial schema.");
                sb.AppendLine("//Note that top left position is [0][0], while bottom right position is [5],[6]");
                sb.AppendLine("//which is different from the Z model in NUS-CS4211/5232.");
                sb.AppendLine("//The state invariants in Z model will be guaranteed by the operations.");
                sb.AppendLine("//");
                sb.AppendLine("//    col number:  0 1 2 3 4 5 6");
                sb.AppendLine("var board[N][M] = [o,o,a,a,o,o,o, //0 row number starting from 0");
                sb.AppendLine("                  o,o,a,a,o,o,o, //1");
                sb.AppendLine("                  a,a,a,w,a,a,a, //2");
                sb.AppendLine("                  a,w,a,a,a,w,a, //3");
                sb.AppendLine("                  o,o,a,w,o,o,o, //4");
                sb.AppendLine("                  o,o,a,a,o,o,o];//5");
                sb.AppendLine("// Black position:");
                sb.AppendLine("var r = 3; //row");
                sb.AppendLine("var c = 0; //column");
                sb.AppendLine("");
                sb.AppendLine("Game = [r-1>=0]MoveUp [] [r-2>=0]PushUp");
                sb.AppendLine("          [] [r+1<N]MoveDown [] [r+2<N]PushDown");
                sb.AppendLine("          [] [c-1>=0]MoveLeft [] [c-2>=0]PushLeft");
                sb.AppendLine("          [] [c+1<M]MoveRight [] [c+2<M]PushRight;");
                sb.AppendLine("");
                sb.AppendLine("MoveUp = [board[r-1][c]==a]go_up{r=r-1} -> Game;");
                sb.AppendLine(
                    "PushUp = [board[r-2][c]==a && board[r-1][c]==w] push_up{board[r-2][c]=w;board[r-1][c]=a;r=r-1;} -> Game;");
                sb.AppendLine("");
                sb.AppendLine("MoveDown = [board[r+1][c]==a]go_down{r=r+1} -> Game;");
                sb.AppendLine(
                    "PushDown = [board[r+2][c]==a && board[r+1][c]==w] push_down{board[r+2][c]=w;board[r+1][c]=a;r=r+1;} -> Game;");
                sb.AppendLine("");
                sb.AppendLine("MoveLeft = [board[r][c-1]==a]go_left{c=c-1} -> Game;");
                sb.AppendLine(
                    "PushLeft = [board[r][c-2]==a && board[r][c-1]==w] push_left{board[r][c-2]=w;board[r][c-1]=a;c=c-1;} -> Game;");
                sb.AppendLine("");
                sb.AppendLine("MoveRight = [board[r][c+1]==a]go_right{c=c+1} -> Game;");
                sb.AppendLine(
                    "PushRight = [board[r][c+2]==a && board[r][c+1]==w] push_right{board[r][c+2]=w;board[r][c+1]=a;c=c+1;} -> Game;");
                sb.AppendLine("");
                sb.AppendLine("");
                sb.AppendLine("#define trouble board[0][3] == w;");
                sb.AppendLine("#define outside board[4][1] == w;");
                sb.AppendLine("#assert Game reaches trouble; //one particular potential trouble position");
                sb.AppendLine("#assert Game reaches outside; //testing if a white can be pushed to ourside");
                sb.AppendLine("");
                sb.AppendLine(
                    "//note: if you change the cross position in the following, the simulator will not reflect the changes.");
                sb.AppendLine(
                    "#define goal board[2][2] == w && board[2][3] == w && board[3][2] == w && board[3][3] == w;");
                sb.AppendLine("#assert Game reaches goal;");
                sb.AppendLine("");
                sb.AppendLine(
                    "#assert Game |= [] (trouble -> !<> goal); //show the trouble position will prevent the goal");
                sb.AppendLine("#assert Game |= [] (trouble -> <> goal);  //testing the impossible");
            }

            return sb.ToString();

        }

        public static string LoadChannelTest()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("channel c 1;");
            sb.AppendLine("var a;");
            sb.AppendLine("var i = 1;");
            sb.AppendLine("");
            sb.AppendLine("main1() =  x() || y() || c!1 -> Stop;");
            sb.AppendLine("");
            sb.AppendLine("main2() =  x() || y() || c!2 -> Stop;");
            sb.AppendLine("");
            sb.AppendLine("main3() =  y() || x() || c!2 -> Stop;");
            sb.AppendLine("");
            sb.AppendLine("main4() =  x() || y() || z();");
            sb.AppendLine("");
            sb.AppendLine("z() = if(i == 1)");
            sb.AppendLine("\t\t   {");
            sb.AppendLine("\t\t   \t\tc!1 -> Skip");
            sb.AppendLine("\t\t   };");
            sb.AppendLine("");
            sb.AppendLine("x() = c?b -> if(b == 1) {t1{a = 1;} -> Skip };");
            sb.AppendLine("");
            sb.AppendLine("y() = c?b -> if(b == 2) {t2{a = 1;} -> Skip };");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("#assert main1 reaches goal;");
            sb.AppendLine("#assert main2 reaches goal;");
            sb.AppendLine("#assert main3 reaches goal;");
            sb.AppendLine("#assert main4 reaches goal;");
            sb.AppendLine("");
            sb.AppendLine("#define goal (a==1);");
            return sb.ToString();
        }
        public static string LoadTest1()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("P(i) = a.i -> P(i);");
            sb.AppendLine("");
            sb.AppendLine("Q = P(0) || P(1);");
            sb.AppendLine("");
            sb.AppendLine("#alphabet P {a.i};");
            sb.AppendLine("");
            sb.AppendLine("#assert Q deadlockfree;");
            return sb.ToString();
        }

        public static string LoadTest2()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("P(i) = a.i -> P(i);");
            sb.AppendLine("");
            sb.AppendLine("Q = P(0) || P(1);");
            sb.AppendLine("");
            sb.AppendLine("#alphabet P {a.x};");
            sb.AppendLine("");
            sb.AppendLine("#assert Q deadlockfree;");
            return sb.ToString();
        }

        public static string LoadTest3()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("P(i) = a.i -> P(i);");
            sb.AppendLine("");
            sb.AppendLine("Q = P(0) || P(1);");
            sb.AppendLine("");
            sb.AppendLine("#alphabet P {a.x};");
            sb.AppendLine("");
            sb.AppendLine("#assert Q deadlockfree;");
            return sb.ToString();
        }

        public static string LoadBakeryAlgorithm1(int N, int BOUND)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("#define N " + N + ";");
            sb.AppendLine("#define BOUND " + BOUND + ";");
            sb.AppendLine("var choosing[N];");
            sb.AppendLine("var number[N];");
            sb.AppendLine("var maxN[N];");
            sb.AppendLine("var critical[N];");
            sb.AppendLine("");
            sb.AppendLine("Process(i)=request.i{choosing[i]=1;}->Hungry(0,i);");
            sb.AppendLine("\t\t\tWait0(0,i);Enter(i);Exit(i);Process(i);");
            sb.AppendLine("");
            sb.AppendLine("Hungry(j,i)=ifa(j>=N)");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\t//Limit the timestamp of processes'requests to enter critical section.");
            sb.AppendLine("\t\t\tifa(maxN[i]<BOUND)");
            sb.AppendLine("\t\t\t{");
            sb.AppendLine("\t\t\t\thungry.i{number[i]=maxN[i]+1;maxN[i]=0;choosing[i]=0;}->Skip");
            sb.AppendLine("\t\t\t}");
            sb.AppendLine("\t\t\telse");
            sb.AppendLine("\t\t\t{");
            sb.AppendLine("\t\t\t\t//If the timerstapm exceeds the BOUND, wait for the process whose request is the latest exits the");
            sb.AppendLine("\t\t\t\t//critical section (set its number to 0)");
            sb.AppendLine("\t\t\t\twaitSomeOneExit.i{maxN[i]=0;}->Hungry(0,i)");
            sb.AppendLine("\t\t\t}");
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t\telse");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\t//on the loop to find the Max of number array");
            sb.AppendLine("\t\t\tifa(number[j] >maxN[i])");
            sb.AppendLine("\t\t\t{");
            sb.AppendLine("\t\t\t\tfindMax.i{maxN[i]=number[j];}->Hungry(j+1,i)");
            sb.AppendLine("\t\t\t}");
            sb.AppendLine("\t\t\telse");
            sb.AppendLine("\t\t\t{");
            sb.AppendLine("\t\t\t\tHungry(j+1,i)");
            sb.AppendLine("\t\t\t}");
            sb.AppendLine("\t\t};");
            sb.AppendLine("\t\t");
            sb.AppendLine("Wait0(j,i)=\tifa(j>=N)");
            sb.AppendLine("\t\t\t{");
            sb.AppendLine("\t\t\t\tSkip");
            sb.AppendLine("\t\t\t}");
            sb.AppendLine("\t\t\telse");
            sb.AppendLine("\t\t\t{");
            sb.AppendLine("\t\t\t\tifa(j==i)");
            sb.AppendLine("\t\t\t\t{");
            sb.AppendLine("\t\t\t\t\tWait0(j+1,i)");
            sb.AppendLine("\t\t\t\t}");
            sb.AppendLine("\t\t\t\telse");
            sb.AppendLine("\t\t\t\t{");
            sb.AppendLine("\t\t\t\t\tWait1(j,i);Wait2(j,i);Wait0(j+1,i)\t");
            sb.AppendLine("\t\t\t\t}");
            sb.AppendLine("\t\t\t};");
            sb.AppendLine("");
            sb.AppendLine("Wait1(j,i)=ifa(choosing[j]==1)");
            sb.AppendLine("\t\t\t{");
            sb.AppendLine("\t\t\t\twaitfor1.i.j->Wait1(j,i)");
            sb.AppendLine("\t\t\t}");
            sb.AppendLine("\t\t\telse");
            sb.AppendLine("\t\t\t{");
            sb.AppendLine("\t\t\t\tSkip");
            sb.AppendLine("\t\t\t};");
            sb.AppendLine("Wait2(j,i)=ifa(number[j]!=0 && (number[j]<number[i]||(number[j]==number[i]&&j<i)))");
            sb.AppendLine("\t\t\t{");
            sb.AppendLine("\t\t\t\twaitfor2.i.j->Wait2(j,i)");
            sb.AppendLine("\t\t\t}");
            sb.AppendLine("\t\t\telse");
            sb.AppendLine("\t\t\t{");
            sb.AppendLine("\t\t\t\tSkip");
            sb.AppendLine("\t\t\t};");
            sb.AppendLine("");
            sb.AppendLine("Enter(i)=entercritical.i{critical[i]=1;}->Skip;");
            sb.AppendLine("Exit(i)=exit.i{number[i]=0;critical[i]=0;}->Skip;");
            sb.AppendLine("");
            sb.AppendLine("Bakery() = |||x :{0..N-1}@Process(x);");
            sb.AppendLine("");
            sb.AppendLine("#assert Bakery()reaches p1;");
            sb.AppendLine("#assert Bakery()reaches p2;");
            sb.AppendLine("");
            sb.AppendLine("#define p1 (" + LoadBakeryAlgorithmUlti1(N, 2) + ");");
            sb.AppendLine("#define p2 (" + LoadBakeryAlgorithmUlti1(N, 1) + ");");
            sb.AppendLine("#define bounded maxN[0] > " + BOUND + ";");
            sb.AppendLine("#assert Bakery() reaches bounded;");
            sb.AppendLine("#assert Bakery() deadlockfree;");
            sb.AppendLine("#assert Bakery |= []( request.0 -> <>entercritical.0); ");
            return sb.ToString();
        }

        //critical[0]+..+critical[N-1]>=n
        private static string LoadBakeryAlgorithmUlti1(int N, int n)
        {
            string result = "(critical[0]";
            for (int i = 1; i < N; i++)
            {
                result += "+ critical[" + i + "]";
            }
            result += ")>=" + n;
            return result;

        }

        public static string LoadTest4()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("var x=0;");
            sb.AppendLine("P()=a->Stop;");
            sb.AppendLine("#define goal (x==0);");
            sb.AppendLine("#assert P() |=<>goal;");
            sb.AppendLine("#assert P() |=X a;");
            return sb.ToString();
        }
        public static string LoadFrogsAndToads(int M, int N)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("//http://www.cut-the-knot.org/SimpleGames/FrogsAndToads.shtml");
            sb.AppendLine("");
            sb.AppendLine("#define M " + M + ";//the number of Frogs, assinged 1 to the square, jump rightward");
            sb.AppendLine("#define N " + N + ";//the number of Toads, assigned 2 to the square, jump backward");
            sb.AppendLine("#define SIZE " + (M + N + 1) + ";//the length of board, SIZE=M+N+1, one square is empty");
            sb.AppendLine("var board=[" + LoadFrogsAndToadsInit(M, N) + "];");
            sb.AppendLine("\t");
            sb.AppendLine("Jump(i)=ifa(board[i]==1&&i+1<SIZE)");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\t\tifa(board[i+1]==0)");
            sb.AppendLine("\t\t\t\t{");
            sb.AppendLine("\t\t\t\t\tfrog.i{board[i]=0;board[i+1]=1;}->Skip");
            sb.AppendLine("\t\t\t\t}");
            sb.AppendLine("\t\t\t\telse");
            sb.AppendLine("\t\t\t\t{");
            sb.AppendLine("\t\t\t\t\tifa(board[i+1]==2&&i+2<SIZE)");
            sb.AppendLine("\t\t\t\t\t{");
            sb.AppendLine("\t\t\t\t\t\tifa(board[i+2]==0)");
            sb.AppendLine("\t\t\t\t\t\t{");
            sb.AppendLine("\t\t\t\t\t\t\tfrog.i{board[i]=0;board[i+2]=1;}->Skip");
            sb.AppendLine("\t\t\t\t\t\t}");
            sb.AppendLine("\t\t\t\t\t\telse");
            sb.AppendLine("\t\t\t\t\t\t{");
            sb.AppendLine("\t\t\t\t\t\t\tSkip");
            sb.AppendLine("\t\t\t\t\t\t}");
            sb.AppendLine("\t\t\t\t\t}");
            sb.AppendLine("\t\t\t\t\telse");
            sb.AppendLine("\t\t\t\t\t{");
            sb.AppendLine("\t\t\t\t\t\tSkip");
            sb.AppendLine("\t\t\t\t\t}");
            sb.AppendLine("\t\t\t\t}");
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t\telse");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tifa(board[i]==2&&i-1>=0)");
            sb.AppendLine("\t\t\t{");
            sb.AppendLine("\t\t\t\tifa(board[i-1]==0)");
            sb.AppendLine("\t\t\t\t{");
            sb.AppendLine("\t\t\t\t\ttoad.i{board[i]=0;board[i-1]=2;}->Skip");
            sb.AppendLine("\t\t\t\t}");
            sb.AppendLine("\t\t\t\telse");
            sb.AppendLine("\t\t\t\t{");
            sb.AppendLine("\t\t\t\t\tifa(board[i-1]==1&&i-2>=0)");
            sb.AppendLine("\t\t\t\t\t{");
            sb.AppendLine("\t\t\t\t\t\ttoad.i{board[i]=0;board[i-2]=2;}->Skip");
            sb.AppendLine("\t\t\t\t\t}");
            sb.AppendLine("\t\t\t\t\telse");
            sb.AppendLine("\t\t\t\t\t{");
            sb.AppendLine("\t\t\t\t\t\tSkip");
            sb.AppendLine("\t\t\t\t\t}");
            sb.AppendLine("\t\t\t\t}");
            sb.AppendLine("\t\t\t}");
            sb.AppendLine("\t\t\telse");
            sb.AppendLine("\t\t\t{");
            sb.AppendLine("\t\t\t\t//empty square");
            sb.AppendLine("\t\t\t\tSkip");
            sb.AppendLine("\t\t\t}");
            sb.AppendLine("\t\t};");
            sb.AppendLine("");
            sb.AppendLine("System()=([]x:{0..SIZE-1}@Jump(x));System;");
            sb.AppendLine("#define goal (" + LoadFrogsAndToadsGoal(M, N) + ");");
            sb.AppendLine("#assert System() reaches goal;");
            return sb.ToString();
        }
        //1,1,1,0,2,2,2
        private static string LoadFrogsAndToadsInit(int M, int N)
        {
            string result = string.Empty;
            for (int i = 1; i <= M + N + 1; i++)
            {
                if (i <= M)
                {
                    result += ",1";
                }
                else if (i == M + 1)
                {
                    result += ",0";
                }
                else
                {
                    result += ",2";
                }
            }
            return result.Trim(',');
        }

        //board[0]==2&&board[1]==2&&board[2]==2&&board[3]==0&&board[4]==1&&board[5]==1&&board[6]==1
        private static string LoadFrogsAndToadsGoal(int M, int N)
        {
            string result = string.Empty;
            for (int i = 1; i <= M + N + 1; i++)
            {
                if (i <= M)
                {
                    result += "&&board[" + (i - 1) + "]==2";
                }
                else if (i == M + 1)
                {
                    result += "&&board[" + (i - 1) + "]==0";
                }
                else
                {
                    result += "&&board[" + (i - 1) + "]==1";
                }
            }
            return result.Trim('&');
        }

        public static string LoadAdding(int MAX, int VAL)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("#define MAX " + MAX + ";");
            sb.AppendLine("var c=1;");
            sb.AppendLine("var x=0;");
            sb.AppendLine("var y=0;");
            sb.AppendLine("");
            sb.AppendLine("P()=if(c<MAX)");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tp1{x=c;}->p2{x=x+c;}->p3{c=x;}->P()");
            sb.AppendLine("\t}");
            sb.AppendLine("\telse");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tStop");
            sb.AppendLine("\t};");
            sb.AppendLine("Q()=if(c<MAX)");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tq1{y=c;}->q2{y=y+c;}->q3{c=y;}->Q()");
            sb.AppendLine("\t}");
            sb.AppendLine("\telse");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tStop");
            sb.AppendLine("\t};");
            sb.AppendLine("");
            sb.AppendLine("System()=P()||Q();");
            sb.AppendLine("#define goal c==" + VAL + ";");
            sb.AppendLine("#assert System() reaches goal;");
            return sb.ToString();
        }

        /// <summary>
        /// This example contains all trivial yet tricky test cases.
        /// </summary>
        /// <returns></returns>
        public static string TrivalTestCases()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("P() = a -> b -> Stop [] c -> Stop");
            sb.AppendLine("#assert P() |= []a;");
            sb.AppendLine("var x=0;");
            sb.AppendLine("Q() = add{x=1;} -> Q()");
            sb.AppendLine("#assert P() |= []a;");
            sb.AppendLine("#define goal x==1;");
            sb.AppendLine("#assert Q() |= []goal;");
            sb.AppendLine("#assert Q() |= []<>goal;");
            return sb.ToString();
        }


        /// <summary>
        /// This example contains all trivial yet tricky test cases.
        /// </summary>
        /// <returns></returns>
        public static string LoadParallelModelChecking(int N, int threads)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("#define N " + N + ";");
            sb.AppendLine("var x = 0;");
            sb.AppendLine();

            sb.Append("P() = a0 -> P0(0)");
            for (int i = 1; i < threads; i++)
            {
                sb.Append("  [] a" + i + " -> P" + i + "(0)");
            }
            sb.AppendLine(";");
            sb.AppendLine();

            for (int i = 0; i < threads; i++)
            {
                sb.AppendLine("P" + i + "(i) = if (i < N) {");
                sb.AppendLine("			sf(dump" + i + ".i){x=1} -> a" + i + "{x=0} -> P" + i + "(i)");
                sb.AppendLine("			[] sf(ha" + i + ".i){x=1} -> P" + i + "(i+1)");
                sb.AppendLine("	   } ");
                sb.AppendLine("	   else {");
                sb.AppendLine("	   		sf(stepdown){x=1} -> P" + i + "(0)");
                sb.AppendLine("	   };");
            }

            sb.AppendLine();
            sb.AppendLine("#define goal x==0;");
            sb.AppendLine("#assert P() |= []<> goal;");

            return sb.ToString();
        }

        /// <summary>
        /// This example contains all trivial yet tricky test cases.
        /// </summary>
        /// <returns></returns>
        public static string LoadParallelModelChecking2(int N, int threads)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("//=======================Model Details===========================");
            sb.AppendLine("#define N " + N + ";");
            sb.AppendLine("#define M " + threads + ";");
            sb.AppendLine("");
            sb.AppendLine("P(i,x) = [x<N]up.i.x -> P(i,x+1)");
            sb.AppendLine("               []");
            sb.AppendLine("          [x>0]down.i.x -> P(i,x-1)");
            sb.AppendLine("               []");
            sb.AppendLine("          [x==N]b.i ->");
            sb.AppendLine("                       if (i < M) {");
            sb.AppendLine("                               P(i+1, 0)");
            sb.AppendLine("                       } else {");
            sb.AppendLine("                               Stop");
            sb.AppendLine("                       };");
            sb.AppendLine("");
            sb.AppendLine("aSys = P(0,0);");
            sb.AppendLine("");
            sb.AppendLine("// SCC ration 0.5");
            sb.AppendLine("#assert aSys |= []<> b." + threads + ";");
            sb.AppendLine("// SCC ration 1");
            sb.AppendLine("#assert aSys |= <> b." + threads + ";");

            return sb.ToString();
        }


        public static string LoadParameterizedReaderWriter(int n, bool isInfinite, bool useProcessCounterVariable)
        {
            StringBuilder sb = new StringBuilder();            

            if (!useProcessCounterVariable)
            {
                sb.AppendLine("//The classic Readers/Writers Example model multiple processes accessing a shared file.");
                sb.AppendLine("");
                sb.AppendLine("////////////////The Model//////////////////");
                sb.AppendLine("//the maximun size of the readers that can read concurrently");
                sb.AppendLine("#define M " + n + ";");
                sb.AppendLine("var writing = false;");
                sb.AppendLine("var noOfReading = 0;");
                sb.AppendLine("");
                sb.AppendLine("Writer() 	= [noOfReading == 0 && !writing]startwrite{writing = true;} -> stopwrite{writing = false;} -> Writer();");
                sb.AppendLine("Reader() 	= [noOfReading < M && !writing]startread{noOfReading = noOfReading+1;} -> ");
                sb.AppendLine("                          //the following guard condition is important to avoid infinite state space, because noOfReading can go negtively infinitely");
                sb.AppendLine("                          ([noOfReading > 0]stopread{noOfReading = noOfReading-1;} -> Reader());");
                sb.AppendLine();
                sb.AppendLine("//there are infinite number of Readers and Writers");
                if (isInfinite)
                {
                    sb.AppendLine("ReadersWriters() = |||{..} @ (Reader() ||| Writer());");
                }
                else
                {
                    sb.AppendLine("ReadersWriters() = |||{M} @ (Reader() ||| Writer());");
                }
                sb.AppendLine("");
                sb.AppendLine("////////////////The Properties//////////////////");
                sb.AppendLine("#assert ReadersWriters() deadlockfree;");
                sb.AppendLine("#define exclusive !(writing == true && noOfReading > 0); ");
                sb.AppendLine("#assert ReadersWriters() |= [] exclusive;");
                sb.AppendLine("#define someonereading noOfReading > 0;");
                sb.AppendLine("#assert ReadersWriters() |= []<>someonereading;");
                sb.AppendLine("#define someonewriting writing == true;");
                sb.AppendLine("#assert ReadersWriters() |= []<>someonewriting;");
                return sb.ToString();
            }
            else
            {
                sb.AppendLine("//The classic Readers/Writers Example model multiple processes accessing a shared file.");
                sb.AppendLine("");
                sb.AppendLine("////////////////The Model//////////////////");
                sb.AppendLine("//the maximun size of the readers that can read concurrently");
                if (!isInfinite)
                {
                    sb.AppendLine("#define M " + n + ";");
                }
                sb.AppendLine("#define noOfReading #Reading();");
                sb.AppendLine("var writing = false;");
                sb.AppendLine("var reading = false;");
                sb.AppendLine("");
                sb.AppendLine("Writer() 	= [noOfReading == 0 && !writing]startwrite{writing = true;} -> stopwrite{writing = false;} -> Writer();");
                sb.AppendLine("Reader() 	= [!writing]startread{reading = true} -> Reading();");
                sb.AppendLine("Reading()    = stopread{if (noOfReading == 1) {reading = false}} -> Reader();");
                sb.AppendLine();
                sb.AppendLine("//there are infinite number of Readers and Writers");
                if (isInfinite)
                {
                    sb.AppendLine("ReadersWriters() = |||{..} @ (Reader() ||| Writer());");
                }
                else
                {
                    sb.AppendLine("ReadersWriters() = |||{M} @ (Reader() ||| Writer());");
                }
                sb.AppendLine("");

                sb.AppendLine("////////////////The Properties//////////////////");
                sb.AppendLine("#assert ReadersWriters() deadlockfree;");
                sb.AppendLine("#define exclusive !(writing == true && reading == true); ");
                sb.AppendLine("#assert ReadersWriters() |= [] exclusive;");
                sb.AppendLine("#define someonereading reading == true;");
                sb.AppendLine("#assert ReadersWriters() |= []<>someonereading;");
                sb.AppendLine("#define someonewriting writing == true;");
                sb.AppendLine("#assert ReadersWriters() |= []<>someonewriting;");
                return sb.ToString();
            }
        }


        public static string LoadParameterizedKValuedRegister(int N, int numberOfreader)
        {
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("//shared binary array of size N");
            sb.Append("var B = [");
            for (int i = 0; i < N - 1; i++)
            {
                sb.Append("0,");
            }
            sb.AppendLine("1];");
            sb.AppendLine();

            sb.AppendLine("////////////////The Concrete Implementation Model//////////////////");
            sb.AppendLine("Readers() = read_inv -> UpScan(0);");

            sb.AppendLine("UpScan(i) =  if(B[i] == 1) { DownScan(i - 1, i) } else { UpScan(i + 1) };");
            sb.AppendLine("DownScan(i, v) =");
            sb.AppendLine("\t\tifa(i >= 0) {");
            sb.AppendLine("\t\t\tif(B[i] == 1) { DownScan(i - 1, i) } else { DownScan(i - 1, v) }");
            sb.AppendLine("\t\t} else {");
            sb.AppendLine("\t\t\tt -> read_res.v -> Readers()");
            sb.AppendLine("\t\t};");

            sb.AppendLine();

            sb.AppendLine("Writer(i) = write_inv.i -> t{B[i] = 1;} -> WriterDownScan(i-1);");
            sb.AppendLine("WriterDownScan(i) = if(i >= 0 ) { t{B[i] = 0;} -> WriterDownScan(i-1) } else { write_res -> Skip } ;");
            sb.AppendLine();

            sb.Append("Writers() = (");
            for (int i = 0; i < N - 1; i++)
            {
                sb.Append("Writer(" + i + ")[]");
            }
            sb.AppendLine("Writer(" + (N - 1) + ")); Writers();");

            if (numberOfreader > 0)
            {
                sb.Append("Register() = ((|||{" + numberOfreader + "}@Readers()) ||| Writers());");
            }
            else
            {
                sb.Append("Register() = ((|||{..}@Readers()) ||| Writers());");
            }

            sb.AppendLine();

            sb.AppendLine("////////////////The Properties//////////////////");
            sb.AppendLine("#assert Register() deadlockfree;");
            sb.AppendLine("#assert Register() |= []<> read_res." + (N - 1) + ";");

            string tmp = "(";
            for (int i = 0; i < N ; i++)
            {
                if(i == N - 1)
                {
                    tmp += "read_res." + i ;
                }
                else
                {
                    tmp += "read_res." + i + " ||";
                }
            }
            tmp += ")";

            sb.AppendLine("#assert Register() |= [](read_inv -> <>" + tmp + ");");
            return sb.ToString();
        }

        public static string LoadParameterizedStack(int n, int processNumber)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("//stack size");
            sb.AppendLine("#define SIZE " + n + ";");
            sb.AppendLine("");
            sb.AppendLine("//shared head pointer for the concrete implementation");
            sb.AppendLine("var H = 0;");
            sb.AppendLine("");
            sb.AppendLine("////////////////The Concrete Implementation Model//////////////////");
            sb.AppendLine("Push() = push_inv -> PushLoop(H);");
            sb.AppendLine("");
            sb.AppendLine("PushLoop(v) = (");
            sb.AppendLine("	ifa (v == H) {");
            sb.AppendLine("		t{if(H < SIZE) {H = H+1;}} -> push_res.(v+1) -> Process()");
            sb.AppendLine("	} else {");
            sb.AppendLine("		Push()");
            sb.AppendLine("	});");
            sb.AppendLine("	");
            sb.AppendLine("Pop() = pop_inv -> 	PopLoop(H);");
            sb.AppendLine("");
            sb.AppendLine("PopLoop(v) = ");
            sb.AppendLine("	(if(v == 0) {");
            sb.AppendLine("		pop_res.0 -> Process() ");
            sb.AppendLine("	} else {");
            sb.AppendLine("		(ifa(v != H) { Pop() } else { t{H = H-1;} -> pop_res.(v-1) -> Process()");
            sb.AppendLine("		})");
            sb.AppendLine("	});");
            sb.AppendLine("");
            sb.AppendLine("Process() = (Push()[]Pop());");
            if (processNumber == 0)
            {
                sb.AppendLine("Stack() = (|||{..}@Process());");
            }
            else
            {
                sb.AppendLine("Stack() = (|||{" + processNumber + "}@Process());");
            }
            sb.AppendLine("");
            sb.AppendLine("#assert Stack() deadlockfree;");

            string tmp = "(";
            for (int i = 0; i < n; i++)
            {
                if (i == n - 1)
                {
                    tmp += "push_res." + i;
                }
                else
                {
                    tmp += "push_res." + i + " ||";
                }
            }
            tmp += ")";

            sb.AppendLine("#assert Stack() |= [](push_inv -> <>" + tmp + ");");

            return sb.ToString();
        }

        public static string LoadParameterizedLeaderElection(int n, bool isInfinite, bool hasProcessCounterVarariable)
        {
            StringBuilder sb = new StringBuilder();

            if (!hasProcessCounterVarariable)
            {
                sb.AppendLine("#define N " + n + ";");
                sb.AppendLine("var detectorcorrect = false;");
                sb.AppendLine("var detector = false;");
                sb.AppendLine("var leaderCount;");
                sb.AppendLine("");
                sb.AppendLine("Node(isLeader) = [isLeader == 1 && leaderCount > 1] rule1.1.1{leaderCount = leaderCount-1;} -> Node(0) ");
                sb.AppendLine("			  [] [isLeader == 0 && leaderCount < N && !((!detectorcorrect && detector) || (detectorcorrect && leaderCount > 0))]rule2.0.0{leaderCount = leaderCount+1;} -> Node(1) ");
                sb.AppendLine("			  [] [isLeader == 0 && leaderCount < N && ((!detectorcorrect && detector) || (detectorcorrect && leaderCount > 0))]rule3.0.0 -> Node(isLeader);");
                sb.AppendLine("			  ");
                sb.AppendLine("/* eventual leader detector*/");
                sb.AppendLine("Detector()  = [!detectorcorrect]((random1{detector = false;} -> Detector()) [] (random2{detector = true;} -> Detector()));");
                sb.AppendLine("Oracle() = progress{detectorcorrect = true;} -> Stop;");
                sb.AppendLine("");
                if (isInfinite)
                {
                    sb.AppendLine("LeaderElection()  = (|||{..} @ Node(0)) ||| Detector() ||| Oracle();");
                    sb.AppendLine("//Where the number of leaders is random initially.");
                    sb.AppendLine("LeaderElectionRandomInit() = []x:{0..N} @ (set{leaderCount = x} -> ((|||{x} @ Node(1)) ||| (|||{..} @ Node(0)) ||| Detector() ||| Oracle()));");
                }
                else
                {
                    sb.AppendLine("LeaderElection()  = (|||{N} @ Node(0)) ||| Detector() ||| Oracle();");
                    sb.AppendLine("//Where the number of leaders is random initially.");
                    sb.AppendLine("LeaderElectionRandomInit() = []x:{0..N} @ (set{leaderCount = x} -> ((|||{x} @ Node(1)) ||| (|||{N-x} @ Node(0)) ||| Detector() ||| Oracle()));");
                }


                sb.AppendLine("");
                sb.AppendLine("////////////////The Properties//////////////////");
                sb.AppendLine("#define oneLeader leaderCount == 1;");
                sb.AppendLine("#assert LeaderElection() |= <>[]oneLeader;");
                sb.AppendLine("#assert LeaderElection() deadlockfree;");
                sb.AppendLine("#assert LeaderElectionRandomInit() |= <>[]oneLeader;");
                sb.AppendLine("#assert LeaderElectionRandomInit() deadlockfree;");

                return sb.ToString();
            }
            else
            {
                if (!isInfinite)
                {
                    sb.AppendLine("#define N " + n + ";");
                }
                sb.AppendLine("#define leaderCount #Node(1);");
                sb.AppendLine("var detectorcorrect = false;");
                sb.AppendLine("var detector = false;");
                sb.AppendLine("var leaderCount1 = false;"); 
                sb.AppendLine("");
                sb.AppendLine("Node(isLeader) = [isLeader == 1 && leaderCount > 1] rule1.1.1{");
			  	sb.AppendLine("		if (leaderCount == 1) {");
			  	sb.AppendLine("			leaderCount1 = false");
			  	sb.AppendLine("		}");
			  	sb.AppendLine("		else {");
			  	sb.AppendLine("			if (leaderCount == 2) {");
			  	sb.AppendLine("				leaderCount1 = true");
			  	sb.AppendLine("			}");
                sb.AppendLine("		}");
			  	sb.AppendLine("	}");                 
                sb.AppendLine("    -> Node(0) ");
                sb.AppendLine("			  [] [isLeader == 0 && !((!detectorcorrect && detector) || (detectorcorrect && leaderCount > 0))]rule2.0.0{");
			  	sb.AppendLine("		if (leaderCount == 0) {");
			  	sb.AppendLine("			leaderCount1 = true");
			  	sb.AppendLine("		}");
			  	sb.AppendLine("		else {");
			  	sb.AppendLine("			if (leaderCount == 1) {");
			  	sb.AppendLine("				leaderCount1 = false");
			  	sb.AppendLine("			}");
                sb.AppendLine("		}");
			  	sb.AppendLine("	}  -> Node(1) ");
                sb.AppendLine("			  [] [isLeader == 0 && ((!detectorcorrect && detector) || (detectorcorrect && leaderCount > 0))]rule3.0.0 -> Node(isLeader);");
                sb.AppendLine("			  ");
                sb.AppendLine("/* eventual leader detector*/");
                sb.AppendLine("Detector()  = [!detectorcorrect]((random1{detector = false;} -> Detector()) [] (random2{detector = true;} -> Detector()));");
                sb.AppendLine("Oracle() = progress{detectorcorrect = true;} -> Stop;");
                sb.AppendLine("");
                if (isInfinite)
                {
                    sb.AppendLine("LeaderElection()  = (|||{..} @ Node(0)) ||| Detector() ||| Oracle();");
                    sb.AppendLine("//Where the number of leaders is random initially.");
                    sb.AppendLine("LeaderElectionRandomInit() = (|||{..} @ Node(1)) ||| (|||{..} @ Node(0)) ||| Detector() ||| Oracle();");
                }
                else
                {
                    sb.AppendLine("LeaderElection()  = (|||{N} @ Node(0)) ||| Detector() ||| Oracle();");
                    sb.AppendLine("//Where the number of leaders is random initially.");
                    sb.AppendLine("LeaderElectionRandomInit() = []x:{0..N} @ ((|||{x} @ Node(1)) ||| (|||{N-x} @ Node(0)) ||| Detector() ||| Oracle());");
                }


                sb.AppendLine("");
                sb.AppendLine("////////////////The Properties//////////////////");
                sb.AppendLine("#define oneLeader leaderCount1 == true;");
                sb.AppendLine("#assert LeaderElection() |= <>[]oneLeader;");
                sb.AppendLine("#assert LeaderElection() deadlockfree;");
                sb.AppendLine("#assert LeaderElectionRandomInit() |= <>[]oneLeader;");
                sb.AppendLine("#assert LeaderElectionRandomInit() deadlockfree;");

                return sb.ToString();
            }
        }


    public static string LoadParameterizedMetaLock(int n, bool isInfinite)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("#define N "+n+"; /* max num of threads that can wait at waiting state */");
        sb.AppendLine();

        sb.AppendLine("/* msg */");
        sb.AppendLine("#define put_fast 100;");
        sb.AppendLine("#define get_fast 101;");
        sb.AppendLine("#define go 102;");
        sb.AppendLine("#define request 103;");
        sb.AppendLine("#define release 104;");
        sb.AppendLine("#define get_slow 105;");
        sb.AppendLine("#define put_slow 106;");
        sb.AppendLine();

        sb.AppendLine("channel ascChan 0;");
        sb.AppendLine("var count =0;");
        sb.AppendLine("var access = false;");
        sb.AppendLine();

        sb.AppendLine("handoff() = ascChan?release -> ascChan?request -> ascChan!go -> handoff()");
        sb.AppendLine("	     [] ascChan?request -> ascChan?release -> ascChan!go -> handoff();");
        sb.AppendLine();

        sb.AppendLine("shared_obj() = ascChan?get_fast -> Busy_0();");
        sb.AppendLine();

        sb.AppendLine("Busy_0() = ascChan?put_fast -> shared_obj()");
        sb.AppendLine("    	[] ascChan?get_slow -> inc{count = count+1;} -> Busy_N();");
        sb.AppendLine();

        if(isInfinite)
        {
            sb.AppendLine("Busy_N() = [count < N]ascChan?get_slow -> inc{count = count+1;} -> Busy_N()");
        }
        else
        {
            sb.AppendLine("Busy_N() = ascChan?get_slow -> inc{count = count+1;} -> Busy_N()");
        }

        sb.AppendLine("	    [] ascChan?put_slow -> dec{count = count -1;} ->");
        sb.AppendLine("        	(");
        sb.AppendLine("		      if(count == 0){");
        sb.AppendLine("			     Busy_0()");
        sb.AppendLine("		      } else {");
        sb.AppendLine("			     Busy_N()");
        sb.AppendLine("		      }");
        sb.AppendLine("	       );");
        sb.AppendLine("	       	       ");
        sb.AppendLine();
        sb.AppendLine("my_thread() = ascChan!get_fast -> enterCritical{access = true;} ->Owner()");
        sb.AppendLine("	       [] ascChan!get_slow -> ascChan!request -> ascChan?go -> enterCritical{access = true;} ->Owner();");
        sb.AppendLine();
        sb.AppendLine("Owner() = leaveCritical{access = false;} ->");
        sb.AppendLine("          (");
        sb.AppendLine("		       ascChan!put_fast -> my_thread()");
        sb.AppendLine("	    	[] ascChan!put_slow -> ascChan!release -> my_thread()");
        sb.AppendLine("	      );");
        sb.AppendLine();

        if(isInfinite)
        {
            sb.AppendLine("System() = ((|||{..}@my_thread())||| handoff() ||| shared_obj());");
        }
        else
        {
            sb.AppendLine("System() = ((|||{N}@my_thread())||| handoff() ||| shared_obj());");
        }

        sb.AppendLine();
        sb.AppendLine("#assert System() deadlockfree;");
        sb.AppendLine("#define someoneaccess access == true;");
        sb.AppendLine("#assert System() |= []<>someoneaccess;");
        return sb.ToString();
    }

    public static string LoadLanguageFeature()
    {
        StringBuilder sb = new StringBuilder();

       // sb.AppendLine("#import \"PAT.Lib.Set\";");
        sb.AppendLine();
        sb.AppendLine("#define D1 2;");
        sb.AppendLine("#define D2 3;");
        sb.AppendLine("");
        sb.AppendLine("//A synchronous channel");
        sb.AppendLine("channel c 0;");
        sb.AppendLine("");
        sb.AppendLine("//An asynchronous channel");
        sb.AppendLine("channel ac 1;");
        sb.AppendLine("");
        sb.AppendLine("//example of multi-dimensional array ");
        sb.AppendLine("var x[D1][D2];");
        sb.AppendLine("//example of multi-dimensional array with initial value");
        sb.AppendLine("var y[D1][D2] = [1..6];");        
        sb.AppendLine("");
        //sb.AppendLine("//example of declaring a set");
        //sb.AppendLine("var set1[0];");
        //sb.AppendLine("var set2 = [2];");
        //sb.AppendLine("");
        //sb.AppendLine("//using sets and test the set library ");
        //sb.AppendLine("Lib() = add{set1=call(SetAdd, set1, 1);} -> add2{set1=call(SetAdd, set1, 2);} -> add3{set1=call(SetAdd, set1, 2);} -> remove{set1=call(SetRemove, set1, 2);} -> ");
        //sb.AppendLine("		if(call(SetContains, set1, 1))");
        //sb.AppendLine("		{");
        //sb.AppendLine("			add22{set2=call(SetAdd, set2, 3);} -> union{set2=call(SetUnion, set1, set2);} -> substract{set2=call(SetSubstract, set2, set1);} -> Lib()			");
        //sb.AppendLine("		};	");
        //sb.AppendLine("");
        sb.AppendLine("//example of channel to send multiple data in one action. An array can be sent in the channel as a whole.");
        sb.AppendLine("//in the channel input events, constanst can be used to make sure that only channel data with matching value are received, otherwise the input event is blocked.");
        sb.AppendLine("P(i) = c!x.i -> ac?u.1 -> P(x);");
        sb.AppendLine("");
        sb.AppendLine("Q(i) = c?z.m -> ");
        sb.AppendLine("	   //the channel communication are pass-by-value, change the value of x will not affect z.");
        sb.AppendLine("	   //although z is recieved as a two dimensional array, but it can only be accessed as a 1 dimensional array. This is a limiation of PAT, since PAT does not store multi-dimentional array internally.");
        sb.AppendLine("	   tau{x[0][1] = 5; x[0][2] = z[1];} ->");
        sb.AppendLine("	   //value of z can be used, but can not be updated.");
        sb.AppendLine("	   ac!z[1].m -> Q(m);");
        sb.AppendLine("");
        sb.AppendLine("aSys1 = P(1) ||| Q(2);"); //Lib() ||| 
        sb.AppendLine("");
        sb.AppendLine("#assert aSys1 deadlockfree;");
        return sb.ToString();
    }

    public static string LoadCSharpExample()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("#import \"PAT.Lib.List\";")
        ;
        sb.AppendLine("#import \"PAT.Lib.Stack\";")
        ;
        sb.AppendLine("#import \"PAT.Lib.Queue\";")
        ;
        sb.AppendLine("#import \"PAT.Lib.Set\";")
        ;
        sb.AppendLine("");
        sb.AppendLine("//test the list library ");
        sb.AppendLine("var<List> list; ");
        sb.AppendLine("var<List> list2;");
        sb.AppendLine("");
        sb.AppendLine("");
        sb.AppendLine("List() = add1{list.Add(1);list2.Add(4);list2.Add(5);} -> add2{list.Add(2);} -> add3{list.Add(3)} -> remove{list.Remove(2);} -> ");
        sb.AppendLine("		if(!list.Contains(2))");
        sb.AppendLine("		{");
        sb.AppendLine("			add22{list.Add(3);} -> concatenate{list= list.Concat(list, list2);} -> List2()			");
        sb.AppendLine("		};	");
        sb.AppendLine("");
        sb.AppendLine("List2() = add1{list.Add(1);}  -> remove{list.Remove(1);} ->remove{list.Remove(2);} -> Skip;");
        sb.AppendLine("");
        sb.AppendLine("//test the stack library ");
        sb.AppendLine("var<Stack> stack1;");
        sb.AppendLine("");
        sb.AppendLine("Stack() =  push1{stack1.Push(1)}-> clear{stack1.Clear()}-> push2{stack1.Push(2)} -> pop1{stack1.Pop()} -> ");
        sb.AppendLine("		   push3{stack1.Push(3)} -> if (3 == stack1.Peek()){if ( (stack1.Contains(3)== true) ) { Stack()} };");
        sb.AppendLine("");
        sb.AppendLine("StackException() =push1{stack1.Push(1)} -> pop1{stack1.Pop()}-> peek{stack1.Peek()} -> StackException();");
        sb.AppendLine("");
        sb.AppendLine("//test the queue library ");
        sb.AppendLine("var<Queue> queue;");
        sb.AppendLine("var<Queue> queue2;");
        sb.AppendLine("");
        sb.AppendLine("Queue() = enter1{queue.Enqueue(1);queue2.Enqueue(2);queue2.Enqueue(3) } -> enter1{queue.Enqueue(2)} ->");
        sb.AppendLine("		  if (1 == queue.First() && 2 == queue.Last())");
        sb.AppendLine("		  {");
        sb.AppendLine("		  	exit{queue.Dequeue()} -> concat{queue = queue.Concat(queue, queue2)}");
        sb.AppendLine("          	-> if (queue.Contains(3)&& 3 == queue.Count()) {Queue() }");
        sb.AppendLine("	      };		");
        sb.AppendLine("");
        sb.AppendLine("//test the set library");
        sb.AppendLine("var<Set> set1;");
        sb.AppendLine("var<Set> set2;");
        sb.AppendLine("var<Set> set3;");
        sb.AppendLine("");
        sb.AppendLine("Set() = initialize{ set1.Add(1);set2.Add(2);set3.Add(3);set3.Add(4)} -> remove3 {set3.Remove(4)} ->");
        sb.AppendLine("		if (set1.IsDisjoint(set2)== true)");
        sb.AppendLine("		{");
        sb.AppendLine("			union12{set3 = set3.Union(set1, set2)} ->");
        sb.AppendLine("			");
        sb.AppendLine("			if (set1.IsOverlapping(set3)== true)");
        sb.AppendLine("			{");
        sb.AppendLine("				substract3{set3 = set3.Substract(set3,set1)} -> add3{set3.Add(6)} ->");
        sb.AppendLine(
            "				 if (set3.Contains(2) == true){intersect23{set3 = set3.Intersect(set3, set2)}-> remove3{set3.Remove(2)} -> Skip}");
        sb.AppendLine("			");
        sb.AppendLine("			}");
        sb.AppendLine("		");
        sb.AppendLine("		};");
        sb.AppendLine("");
        sb.AppendLine("");
        sb.AppendLine("#assert List() deadlockfree;");
        sb.AppendLine("#assert Stack() deadlockfree;");
        //sb.AppendLine("#assert StackException() deadlockfree;");
        sb.AppendLine("#assert Queue() deadlockfree;");
        sb.AppendLine("#assert Set() deadlockfree;");
        return sb.ToString();
    }


        public static string LoadSynchronousChannel0()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("channel c 0;");
            sb.AppendLine("channel c1 0;");

            sb.AppendLine("main1() =  c!1 -> c1?x -> Stop;");
            sb.AppendLine("main2() =  c?x -> c1!x -> Stop;");
            sb.AppendLine("main3() =  c!2 -> c1?x -> Stop;");
            sb.AppendLine("Main = main1() ||| main2() ||| main3();");

            sb.AppendLine("#assert Main deadlockfree;");

            return sb.ToString();
        }

        public static string LoadSynchronousChannel1()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("channel c 0;");
            sb.AppendLine("channel c1 0;");

            sb.AppendLine("main1() =  c!1 -> c1?x -> main1;");
            sb.AppendLine("main2() =  c?x -> c1!x -> main2;");
            sb.AppendLine("main3() =  c!2 -> c1?x -> main3;");
            sb.AppendLine("Main = main1() ||| main2() ||| main3();");

            sb.AppendLine("#assert Main deadlockfree;");

            return sb.ToString();
        }

        public static string LoadSynchronousChannel2()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("channel ch0 0;");
            sb.AppendLine("channel ch1 1;");
            sb.AppendLine("var x = 5;");

            sb.AppendLine("P() = ch1!x -> ch0?z -> P();");
            sb.AppendLine("Q() = ch1?y -> ch0!y -> Q();");
            sb.AppendLine("ASystem() = P() || Q();");

            sb.AppendLine("#assert ASystem() deadlockfree;");

            return sb.ToString();
        }


        /// <summary>
        /// This example is the classic Dining Philosophiers. 
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public static string LoadFischerProtocol(int N)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("#define N " + N + ";");
            sb.AppendLine("#define Delta 3;");
            sb.AppendLine("#define Epsilon 4;");
            sb.AppendLine("#define Idle -1;");
            sb.AppendLine("");
            sb.AppendLine("var x = Idle;");
            sb.AppendLine("var counter;");
            sb.AppendLine("");
            sb.AppendLine("//timed version");
            sb.AppendLine("P(i) = ifb(x == Idle) {");
            sb.AppendLine("	 	((update.i{x = i} -> Wait[Epsilon]) within[Delta]);");
            sb.AppendLine("			if (x == i) {");
            sb.AppendLine("				cs.i{counter++} -> exit.i{counter--; x=Idle} -> P(i)");
            sb.AppendLine("		   } else {");
            sb.AppendLine("		        P(i)");
            sb.AppendLine("		   }");
            sb.AppendLine("	   };");
            //sb.AppendLine("	   else");
            //sb.AppendLine("	   {");
            //sb.AppendLine("	   	  P(i)	");
            //sb.AppendLine("	   };");
            sb.AppendLine("		    ");
            sb.AppendLine("FischersProtocol = ||| i:{0..N-1}@P(i);	");
            sb.AppendLine("");
            sb.AppendLine("//untimed version");
            sb.AppendLine("uP(i) = ifb(x == Idle) {");
            sb.AppendLine("			update.i{x = i} -> ");
            sb.AppendLine("		    if (x == i) {");
            sb.AppendLine("				cs.i{counter++} -> exit.i{counter--; x=Idle} -> uP(i)");
            sb.AppendLine("		    } else {");
            sb.AppendLine("		        uP(i)");
            sb.AppendLine("		    }");
            sb.AppendLine("	   };");
            //sb.AppendLine("	   else");
            //sb.AppendLine("	   {");
            //sb.AppendLine("	   	  uP(i)	");
            //sb.AppendLine("	   };");
            sb.AppendLine("uFischersProtocol = ||| i:{0..N-1}@uP(i);	");
            sb.AppendLine("FischersProtocolDiv = ||| i:{0..N-1}@(P(i) \\ {update.i, cs.i, exit.i});	");
            sb.AppendLine("");
            sb.AppendLine("#assert FischersProtocol deadlockfree;");
            sb.AppendLine("#assert FischersProtocolDiv divergencefree;");
            sb.AppendLine("#assert FischersProtocolDiv divergencefree<T>;");

            sb.AppendLine("");
            sb.AppendLine("//verifying mutual exclusion by reachability analysis");
            sb.AppendLine("#define MutualExclusionFail counter > 1;");
            sb.AppendLine("#assert FischersProtocol reaches MutualExclusionFail;");
            sb.AppendLine("#assert FischersProtocol refines uFischersProtocol;");
            sb.AppendLine("#assert uFischersProtocol refines FischersProtocol;");
            sb.AppendLine();
            sb.AppendLine("//verifying mutual exclusion by trace refinement checking");
            sb.AppendLine("MutualExclusionProcess = Relevant ||| Irrelevant;");
            string temp = "Relevant = ";
            for (int i = 0; i < N; i++)
            {
                temp += "cs." + i + " -> exit." + i + " -> Relevant";

                if (i != N - 1)
                {
                    temp += " [] ";
                }
            }
            temp += ";";

            sb.AppendLine(temp);
            temp = "Irrelevant = ";
            for (int i = 0; i < N; i++)
            {
                temp += "update." + i + " -> Irrelevant";

                if (i != N - 1)
                {
                    temp += " [] ";
                }
            }
            temp += ";";

            sb.AppendLine(temp);
            sb.AppendLine("#assert FischersProtocol refines MutualExclusionProcess;");
            sb.AppendLine("#assert FischersProtocol refines<T> MutualExclusionProcess;");
            sb.AppendLine("");
            sb.AppendLine("//verifying responsiveness by LTL model checking");
            sb.AppendLine("#define request x != Idle;");
            sb.AppendLine("#define accessCS counter > 0;");
            sb.AppendLine("");
            sb.AppendLine("#assert FischersProtocol |= [](request -> <>accessCS);");
            sb.AppendLine("#assert FischersProtocol |= [](update.0 -> <>cs.0);");
            sb.AppendLine("");
            sb.AppendLine("//verifying bounded responsiveness by refinement checking");
            temp = "Bounded = update.0 -> Started [] cs.0 -> Bounded [] exit.0 -> Bounded";
            for (int i = 1; i < N; i++)
            {
                temp += "\n           ";
                temp += " [] update." + i + " -> Bounded [] cs." + i + " -> Bounded [] exit." + i + " -> Bounded";
            }
            sb.AppendLine(temp + ";");
            sb.AppendLine("Started = Working deadline[20]; Bounded;");
            temp = "Working = update.0 -> Working [] cs.0 -> Skip [] exit.0 -> Working";
            for (int i = 1; i < N; i++)
            {
                temp += "\n           ";
                temp += " [] update." + i + " -> Working [] cs." + i + " -> Working [] exit." + i + " -> Working";
            }
            sb.AppendLine(temp + ";");
            sb.AppendLine("#assert FischersProtocol refines<T> Bounded;");
            return sb.ToString();
        }

        /// <summary>
        /// This example is the classic Dining Philosophiers. 
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public static string LoadRailwayCrosing()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Train = trainnear -> nearind -> entercrossing -> leavecrossing -> outind -> Train;");
            sb.AppendLine("Controller = (nearind -> downcommand -> confirm -> Controller) [] (outind -> upcommand -> confirm -> Controller);");
            sb.AppendLine("Gate =  (downcommand -> down -> confirm -> Gate) [] (upcommand -> up -> confirm -> Gate);");
            sb.AppendLine("Crossing = Controller || Gate;");
            sb.AppendLine("System = Train || Crossing;");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("TTrain = trainnear -> nearind ->Wait[300]; entercrossing -> Wait[20]; leavecrossing -> outind -> TTrain;");
            sb.AppendLine("TController = (nearind -> Wait[1]; downcommand -> confirm -> TController) [] (outind ->Wait[1]; upcommand -> confirm -> TController);");
            sb.AppendLine("TGate =  (downcommand -> Wait[100]; down -> confirm -> TGate) [] (upcommand -> Wait[100]; up -> confirm -> TGate);");
            sb.AppendLine("TCrossing = TController || TGate;");
            sb.AppendLine("TSystem = TTrain || TCrossing;");
            sb.AppendLine("");
            sb.AppendLine("#assert System deadlockfree;");
            sb.AppendLine("#assert TSystem deadlockfree;");
            sb.AppendLine("#assert TSystem refines System;");
            sb.AppendLine("#assert System refines TSystem;");
            sb.AppendLine("#assert TSystem refines<F> System;");
            sb.AppendLine("#assert TSystem refines<FD> System;");
            sb.AppendLine("#assert System refines<F> TSystem;");
            sb.AppendLine("#assert System refines<FD> TSystem;");
            return sb.ToString();
        }


        /// <summary>
        /// This example is the classic Dining Philosophiers. 
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public static string LoadTimedBridgeCrosing()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/*");
            sb.AppendLine(" * Four vikings are about to cross a damaged bridge in the middle of the");
            sb.AppendLine(" * night. The bridge can only carry two of the vikings at the time and to");
            sb.AppendLine(" * find the way over the bridge the vikings need to bring a torch.  The");
            sb.AppendLine(" * vikings need 5, 10, 20 and 25 minutes (one-way) respectively to cross");
            sb.AppendLine(" * the bridge.");
            sb.AppendLine("");
            sb.AppendLine(" * Does a schedule exist which gets all four vikings over the bridge");
            sb.AppendLine(" * within 60 minutes?");
            sb.AppendLine(" */");
            sb.AppendLine("");
            sb.AppendLine("////////////////The Model//////////////////");
            sb.AppendLine("#define KNIGHT 5;");
            sb.AppendLine("#define LADY 10;");
            sb.AppendLine("#define KING 20;");
            sb.AppendLine("#define QUEEN 25;");
            sb.AppendLine("");
            sb.AppendLine("var Knight = 0;");
            sb.AppendLine("var Lady = 0;");
            sb.AppendLine("var King = 0;");
            sb.AppendLine("var Queen = 0;");
            sb.AppendLine("");
            sb.AppendLine("South() = if (Knight == 0 && Lady == 0) {Wait[LADY]; go_knight_lady{Knight = 1; Lady = 1;} -> North()} else {South}");
            sb.AppendLine("          []");
            sb.AppendLine("          if (Knight == 0 && King == 0) {Wait[KING]; go_knight_king{Knight = 1; King= 1;} -> North()} else {South}");
            sb.AppendLine("          []");
            sb.AppendLine("          if (Knight == 0 && Queen == 0) {Wait[QUEEN]; go_knight_queen{Knight = 1; Queen= 1;} -> North()} else {South}");
            sb.AppendLine("          []");
            sb.AppendLine("          if (Lady == 0 && King == 0) {Wait[KING]; go_lady_king{Lady = 1; King= 1;} -> North()} else {South}");
            sb.AppendLine("          []");
            sb.AppendLine("          if (Lady == 0 && Queen == 0) {Wait[QUEEN]; go_lady_queen{Lady = 1; Queen= 1;} -> North()} else {South}");
            sb.AppendLine("          []");
            sb.AppendLine("          if (King == 0 && Queen == 0) {Wait[QUEEN]; go_king_queen{King = 1; Queen= 1;} -> North()} else {South}");
            sb.AppendLine("          []");
            sb.AppendLine("          if (Knight == 0) {Wait[KNIGHT]; go_knight{Knight = 1;} -> North()} else {South}");
            sb.AppendLine("          []");
            sb.AppendLine("          if (Lady == 0) {Wait[LADY]; go_lady{Lady = 1;} -> North()} else {South}");
            sb.AppendLine("          []");
            sb.AppendLine("          if (King == 0) {Wait[KING]; go_king{King = 1;} -> North()} else {South}");
            sb.AppendLine("          []");
            sb.AppendLine("          if (Queen == 0) {Wait[QUEEN]; go_queen{Queen = 1;} -> North()} else {South};");
            sb.AppendLine("");
            sb.AppendLine("North() = if (Knight == 1 && Lady == 1) {Wait[LADY]; back_knight_lady{Knight = 0; Lady = 0;} -> South()} else {North()}");
            sb.AppendLine("          []");
            sb.AppendLine("          if (Knight == 1 && King == 1) {Wait[KING]; back_knight_king{Knight = 0; King = 0;} -> South()} else {North()}");
            sb.AppendLine("          []");
            sb.AppendLine("          if (Knight == 1 && Queen == 1) {Wait[QUEEN]; back_knight_queen{Knight = 0; Queen = 0;} -> South()} else {North()}");
            sb.AppendLine("          []");
            sb.AppendLine("          if (Lady == 1 && King == 1) {Wait[KING]; back_lady_king{Lady = 0; King = 0;} -> South()} else {North()}");
            sb.AppendLine("          []");
            sb.AppendLine("          if (Lady == 1 && Queen == 1) {Wait[QUEEN]; back_lady_queen{Lady = 0; Queen = 0;} -> South()} else {North()}");
            sb.AppendLine("          []");
            sb.AppendLine("          if (King == 1 && Queen == 1) {Wait[QUEEN]; back_king_queen{King = 0; Queen = 0;} -> South()} else {North()}");
            sb.AppendLine("          []");
            sb.AppendLine("          if (Knight == 1) {Wait[KNIGHT]; back_knight{Knight = 0;} -> South()} else {North()}");
            sb.AppendLine("          []");
            sb.AppendLine("          if (Lady == 1) {Wait[LADY]; back_lady{Lady = 0;} -> South()} else {North()}");
            sb.AppendLine("          []");
            sb.AppendLine("          if (King == 1) {Wait[KING]; back_king{King = 0;} -> South()} else {North()}");
            sb.AppendLine("          []");
            sb.AppendLine("          if (Queen == 1) {Wait[QUEEN]; back_queen{Queen = 0;} -> South()} else {North()};");
            sb.AppendLine("");
            sb.AppendLine("Game59() = South() deadline[59];");
            sb.AppendLine("Game60() = South() deadline[60];");
            sb.AppendLine("////////////////The Properties//////////////////");
            sb.AppendLine("#define goal (Knight==1 && Lady==1 && King==1 && Queen==1);");
            sb.AppendLine("#assert Game59() reaches goal;");
            sb.AppendLine("#assert Game60() reaches goal;");
            return sb.ToString();
        }


        public static string LoadTrainWayControl(int N)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("#import \"PAT.Lib.Queue\";");
            sb.AppendLine("");
            sb.AppendLine("//number of trains");
            sb.AppendLine("#define N " + N + ";");

            sb.AppendLine("");
            sb.AppendLine("channel appr 0;");
            sb.AppendLine("channel go 0;");
            sb.AppendLine("channel leave 0;");
            sb.AppendLine("channel stop 0;");
            sb.AppendLine("");
            sb.AppendLine("var<Queue> queue; ");
            sb.AppendLine("");
            sb.AppendLine("//Train model with 'within'");
            sb.AppendLine("Train(i) = appr!i -> ((Wait[10, 20]; Cross(i)) [] (stop?i  -> StopS(i) within[10]));");
            sb.AppendLine("");
            sb.AppendLine("Cross(i) = (leave!i -> Train(i)) within[3,5];");
            sb.AppendLine("");
            sb.AppendLine("StopS(i) = go?i -> Start(i);");
            sb.AppendLine("");
            sb.AppendLine("Start(i) = Wait[7, 15] ; Cross(i);");
            sb.AppendLine(" ");
            sb.AppendLine("Gate = if (queue.Count() ==0) {");
            sb.AppendLine("		    atomic{appr?i ->  tau{queue.Enqueue(i)} -> Skip}; Occ");
            sb.AppendLine("	   } else {  ");
            sb.AppendLine("	        (go!queue.First() -> Occ) within[10]");
            sb.AppendLine("    };");
            sb.AppendLine(" ");
            sb.AppendLine("Occ = (atomic{leave?[i == queue.First()]i -> tau{queue.Dequeue()} -> Skip}; Gate)");
            sb.AppendLine("     []");
            sb.AppendLine("     (atomic{appr?i -> tau{queue.Enqueue(i)} -> stop!queue.Last() -> Skip}; Occ);");
            sb.AppendLine("     ");
            sb.AppendLine("System = (||| x:{0..N-1}@Train(x)) ||| Gate;");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("#assert System deadlockfree;");
            sb.AppendLine("");
            sb.AppendLine("//Whenever a train approaches the bridge, it will eventually cross.");
            sb.AppendLine("#assert System |= [](appr.1 -> <>leave.1);");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("//verifying bounded responsiveness by refinement checking");
            string temp = "Bounded = appr.0 -> Started [] go.0 -> Bounded [] leave.0 -> Bounded [] stop.0 -> Bounded";
            for (int i = 1; i < N; i++)
            {
                temp += "\n           ";
                temp += " [] appr." + i + " -> Bounded [] go." + i + " -> Bounded [] leave." + i + " -> Bounded [] stop." + i + " -> Bounded";
            }
            sb.AppendLine(temp + ";");
            sb.AppendLine("Started = Working deadline[20]; Bounded;");
            temp = "Working = appr.0 -> Working [] go.0 -> Working [] leave.0 -> Skip [] stop.0 -> Working";
            for (int i = 1; i < N; i++)
            {
                temp += "\n           ";
                temp += " [] appr." + i + " -> Working [] go." + i + " -> Working [] leave." + i + " -> Working [] stop." + i + " -> Working";
            }
            sb.AppendLine(temp + ";");
            sb.AppendLine("#assert System refines<T> Bounded;");
            sb.AppendLine("//overflow: There can never be N elements in the queue (thus the array will not overflow).");
            sb.AppendLine("#define overflow (queue.Count()> N);");
            sb.AppendLine("#assert System reaches overflow;");
            sb.AppendLine("");

            return sb.ToString();

        }

        public static string LoadCSMACD_RTS(int N)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/*");
            sb.AppendLine(" * In a broadcast network with a multi-access bus, the problem of assigning the bus to only one of many competing stations arises.");
            sb.AppendLine(" * The CSMA/CD protocol (Carrier Sense, Multiple-Access with Collision Detection) describes one solution.");
            sb.AppendLine(" * Roughly, whenever a station has data to send, it first listens to the bus.");
            sb.AppendLine(" * If the bus is idle (i.e., no other station is transmitting), the station begins to send a message.");
            sb.AppendLine(" * If it detects a busy bus in this process, it waits a random amount of time and then repeats the operation.");
            sb.AppendLine(" * When a collision occurs, because several stations transmit simultaneously, then all of them detect it,");
            sb.AppendLine(" * abort their transmissions immediately and wait a random time to start all over again.");
            sb.AppendLine("");
            sb.AppendLine(" * We suppose that the bus is a 10Mbps Ethernet with a worst case propagation delay of 26µsec.");
            sb.AppendLine(" * Messages have a fixed length of 1024 bytes and so the time to send a complete message including the propagation delay is 808 µsec.");
            sb.AppendLine(" * For simplicity we make the following assumptions the bus is error free");
            sb.AppendLine(" * only the transmission of messages is modeled and no buffering of incoming messages is allowed.");
            sb.AppendLine("*/");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("#define N " + N + "; //Number of Stations");
            sb.AppendLine("");
            sb.AppendLine("channel begin 0; //Sender starts sending the messages");
            sb.AppendLine("channel busy 0; //the bus is sensed busy by Sender");
            sb.AppendLine("channel end 0; //Sender completes the transmission");
            sb.AppendLine("channel cd 0; //a collision is detected by Sender");
            sb.AppendLine("channel newMess 0; // a message is fetched by Sender");
            sb.AppendLine("");
            sb.AppendLine("//Model Sender");
            sb.AppendLine("//WaitFor(i): Sender(i) is waiting for a message from the upper level");
            sb.AppendLine("WaitFor(i) = (cd?i -> WaitFor(i)) [] (newMess!i -> ((begin!i -> Trans(i)) [] (cd?i -> Retry(i)) [] (busy?i -> Retry(i))));");
            sb.AppendLine("//Trans(i): Sender(i) is sending a message");
            sb.AppendLine("Trans(i) = (cd?i -> Retry(i) within[0, 52]) [] (atomic{Wait[808]; end!i -> Skip};WaitFor(i));");
            sb.AppendLine("//Retry(i): Sender is waiting to retry after detecting a collision or a busy bus");
            sb.AppendLine("Retry(i) = (newMess!i -> ((begin!i -> Trans(i) within[0, 52]) [] (busy?i -> Retry(i) within [0, 52]) [] (cd?i -> Retry(i) within[0, 52])))");
            sb.AppendLine("            [] (cd?i -> Retry(i));");
            sb.AppendLine("");
            sb.AppendLine("//Model Bus");
            sb.AppendLine("//Idle: No station is transmitting");
            sb.AppendLine("Idle = newMess?i -> begin?i -> Active;");
            sb.AppendLine("//Active: Some station has started a transmission");
            sb.AppendLine("Active = (end?i -> Idle) [] (newMess?i-> ((begin?i -> Collision within[0, 26]) [] (atomic{Wait[26]; busy!i -> Skip}; Active)));");
            sb.AppendLine("//Collison: the burst noise of a collision is being propagated");
            sb.AppendLine("Collision = atomic{BroadcastCD(0)} within[0,26]; Idle;");
            sb.AppendLine("					");
            sb.AppendLine("//Broadcast collision signal to every sender					");
            sb.AppendLine("BroadcastCD(x) = if (x < N) {");
            sb.AppendLine("						cd!x -> BroadcastCD(x+1)");
            sb.AppendLine("					}");
            sb.AppendLine("					else {");
            sb.AppendLine("						Skip");
            sb.AppendLine("					};");
            sb.AppendLine("");
            sb.AppendLine("CSMACD=(|||x :{0..N-1}@WaitFor(x))|||Idle;");
            sb.AppendLine("");
            sb.AppendLine("#assert CSMACD deadlockfree;");
            sb.AppendLine("");
            sb.AppendLine("//two stations can not transmit simultaneously after one has listened to the bus for more than 52 µsec");
            sb.AppendLine("SpecProcess = (newMess.0 -> begin.0 -> Constrained1)  // if begin.0 occurs, then constrained1 happens");
            sb.AppendLine("	   [] (newMess.1 -> begin.1 -> (");
            sb.AppendLine("	   		newMess.0 -> begin.0 -> Constrained2 // if begin.1 occurs and then followed by begin.0, then constrained2 happens");
            sb.AppendLine("	   		[]");
            sb.AppendLine("	   		Relaxed //otherwise, no constrain applies.");
            sb.AppendLine("	   )) ");
            sb.AppendLine("	   [] Relaxed; ");
            sb.AppendLine("");
            sb.AppendLine("//constrain1 states that given that begin.0 has occured, if begin.1 occurs, cd.0 or cd.1 must occur within 52 second ");
            sb.AppendLine("Constrained1 = ((newMess.1 -> begin.1 -> ((cd.0 -> Skip [] cd.1 -> Skip) deadline[52])); SpecProcess) [] Relaxed;");
            sb.AppendLine("//constrain1 states that given that begin.1 and begin.0 have occured, cd.0 or cd.1 must occur within 52 second ");
            sb.AppendLine("Constrained2 = (cd.0 -> Skip [] cd.1 -> Skip) deadline[52]; Relaxed;");
            sb.AppendLine("//the following process states that other than the above, the rest of the system is not constrained.");
            sb.AppendLine("");
            sb.AppendLine("Relaxed = ([] x:{2..N-1} @ (newMess.x -> begin.x -> SpecProcess)) []");
            sb.AppendLine("			([] x:{0..N-1} @ ((newMess.x -> (busy.x -> SpecProcess [] cd.x -> SpecProcess))[] (cd.x -> SpecProcess)");
            sb.AppendLine("	   		[] (end.x -> SpecProcess) )");
            sb.AppendLine("	   		); ");
            sb.AppendLine("");
            sb.AppendLine("#assert CSMACD refines<T> SpecProcess;");

            return sb.ToString();

        }

        public static string LoadFDDI(int N)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/*");
            sb.AppendLine(" * Fiber Distributed Data Interface(FDDI) is a high-performance fiber-optic");
            sb.AppendLine(" * token ring Local Area Network. An FDDI network is composed by N identical");
            sb.AppendLine(" * stations and a ring, where the stations can communicate by synchronous");
            sb.AppendLine(" * messages with high priority or synchronous messages with low priority.");
            sb.AppendLine(" */");
            sb.AppendLine("");
            sb.AppendLine("#define N " + N + "; // number of stations");
            sb.AppendLine("#define td 0; //no delay in the token passing of the ring");
            sb.AppendLine("#define TRTT 170; //Target token Rotation Timer");
            sb.AppendLine("#define SA 20; //time units for synchronous transmission");
            sb.AppendLine("");
            sb.AppendLine("var THT[N]; // determine asynchronization messages transmission");
            sb.AppendLine("var TRT[N]; // token rotation time");
            sb.AppendLine("var async_t;");
            sb.AppendLine("var y;");
            sb.AppendLine("");
            sb.AppendLine("channel tt 0; // synchronization token transaction between station and ring");
            sb.AppendLine("channel rt 0; // synchronization release transaction between station and ring");
            sb.AppendLine("");
            sb.AppendLine("// Model Ring ");
            sb.AppendLine("Ring_to(i) = (tt!i -> Ring(i) within [0, td]); // send token to station i");
            sb.AppendLine("Ring(i) = rt?[x == i]x -> Ring_to((i+1) % N); // when receives release message of station i, pass the token to next station");
            sb.AppendLine("");
            sb.AppendLine("// Model Symetric Station");
            sb.AppendLine("// Station i is waiting for the token");
            sb.AppendLine("Station_idle(i) =  tt?[x == i]x -> (sync_mess.i{THT[i] = TRT[i]; TRT[i] = 0; ");
            sb.AppendLine("			 while(y < N) {");
            sb.AppendLine(" 				 TRT[y] = TRT[y] + SA; ");
            sb.AppendLine("				 y = y + 1;");
            sb.AppendLine(" 				 }; y = 0;} -> Station_sync(i) within[0, SA]);");
            sb.AppendLine("");
            sb.AppendLine("// Station i is in possesion of the token an executing the synchronous transmission");
            sb.AppendLine("Station_sync(i) = if (THT[i] >= TRTT) {");
            sb.AppendLine("						end_sync_mess.i -> rt!i -> Station_idle(i)");
            sb.AppendLine("					} else {");
            sb.AppendLine("						end_sync_mess.i{async_t = TRTT - THT[i];} -> Station_async(i)");
            sb.AppendLine("					};");
            sb.AppendLine("");
            sb.AppendLine("// Station i is in possesion of the token an executing the asynchronous transmission");
            sb.AppendLine("Station_async(i) = atomic{Wait[async_t]; async_mess.i -> Skip}; end_async_mess.i{");
            sb.AppendLine("					while (y < N) {");
            sb.AppendLine("						TRT[y] = TRT[y] + async_t; ");
            sb.AppendLine("						y = y + 1;");
            sb.AppendLine("						}; y = 0;} -> rt!i -> Station_idle(i); ");
            sb.AppendLine("");
            sb.AppendLine("System = (|||x :{0..N-1}@Station_idle(x)) ||| Ring_to(0);");
            sb.AppendLine("");
            sb.AppendLine("#assert System deadlockfree;");
            sb.AppendLine("");
            sb.AppendLine("// Safety property: the token is not at two places at the same time");
            sb.AppendLine("#assert System |= []!((end_sync_mess.0 || end_async_mess.0) && (end_sync_mess.1 || end_async_mess.1) && (end_sync_mess.2 || end_async_mess.2));");
            return sb.ToString();

        }

        //***probability
         public static string LoadProbabilityModel1()
         {
             StringBuilder sb = new StringBuilder();
             sb.AppendLine("//=======================Model Details===========================");
             sb.AppendLine("//A simple probabilistic model");
             sb.AppendLine("//found in the book \"Principles of Model Checking\" page 855");
             sb.AppendLine("var current = 0;");
             sb.AppendLine("");
             sb.AppendLine("aSystem = State0;");
             sb.AppendLine("State0 = e{current=1} -> State1");
             sb.AppendLine("              []");
             sb.AppendLine("              pcase {");
             sb.AppendLine("                     [0.33] : e033{current=2} -> State2");
             sb.AppendLine("                     default : e067{current=4} -> State4");
             sb.AppendLine("              };");
             sb.AppendLine("State1 = pcase {");
             sb.AppendLine("                     [0.11] : e011{current=3} -> State3");
             sb.AppendLine("                     [0.5] : e05{current=1} -> State1");
             sb.AppendLine("                     default : e039{current=2} -> State2");
             sb.AppendLine("              };");
             sb.AppendLine("State2 = e -> State2;");
             sb.AppendLine("State3 = e -> State3;");
             sb.AppendLine("State4 = pcase {");
             sb.AppendLine("                     [0.25] : e025{current=5} -> State5");
             sb.AppendLine("                     default : e075{current=6} -> State6");
             sb.AppendLine("              };");
             sb.AppendLine("State5 = e{current=6} -> State6");
             sb.AppendLine("              []");
             sb.AppendLine("              pcase {");
             sb.AppendLine("                     [0.33] : e033{current=2} -> State2");
             sb.AppendLine("                     default : e067{current=7} -> State7");
             sb.AppendLine("              };");
             sb.AppendLine("State6 = pcase {");
             sb.AppendLine("                     [0.25] : e025{current=5} -> State5");
             sb.AppendLine("                     default : e075{current=6} -> State6");
             sb.AppendLine("              };");
             sb.AppendLine("State7 = pcase {");
             sb.AppendLine("                     [0.5] : e05{current=2} -> State2");
             sb.AppendLine("                     default : e05{current=3} -> State3");
             sb.AppendLine("              };");
             sb.AppendLine("");
             sb.AppendLine("#define predicate current == 3;");
             sb.AppendLine("#assert aSystem reaches predicate with pmax;");
             sb.AppendLine("#assert aSystem reaches predicate with pmin;");
             sb.AppendLine("#assert aSystem |= []predicate with pmax;");
             sb.AppendLine("#assert aSystem |= []predicate with pmin;");
             sb.AppendLine("#assert aSystem |= []<>predicate with pmax;");
             sb.AppendLine("#assert aSystem |= []<>predicate with pmin;");
             sb.AppendLine("");
             sb.AppendLine("#assert aSystem deadlockfree;//used to check the number of states in initial model");
             return sb.ToString();

         }

         public static string LoadProbabilityModel2()
         {
             StringBuilder sb = new StringBuilder();
             sb.AppendLine("//=======================Model Details===========================");
             sb.AppendLine("var current = 0;");
             sb.AppendLine("");
             sb.AppendLine("aSystem = State0;");
             sb.AppendLine("State0 =      pcase {");
             sb.AppendLine("                     [0.5] : e05{current=1} -> State1");
             sb.AppendLine("                     default : e05{current=2} -> State2");
             sb.AppendLine("                     }");
             sb.AppendLine("              []");
             sb.AppendLine("              pcase {");
             sb.AppendLine("                     [0.25] : e025{current=3} -> State3");
             sb.AppendLine("                     default : e075{current=2} -> State2");
             sb.AppendLine("              };");
             sb.AppendLine("State1 = pcase {");
             sb.AppendLine("                     [0.5] : e05{current=0} -> State0");
             sb.AppendLine("                     default : e05{current=3} -> State3");
             sb.AppendLine("              };");
             sb.AppendLine("State2 = e -> State2;");
             sb.AppendLine("State3 = e -> State3;");
             sb.AppendLine("");
             sb.AppendLine("#define predicate current == 2;");
             sb.AppendLine("#assert aSystem reaches predicate with pmax;///////should be 0.75");
             sb.AppendLine("#assert aSystem reaches predicate with pmin;///////should be 0.67");
             sb.AppendLine("#assert aSystem |= []<>predicate with pmax;////////should be 0.75(same as reache state2)");
             sb.AppendLine("#assert aSystem |= []<>predicate with pmin;////////should be 0.67(same as reache state2)");
             sb.AppendLine("");
             sb.AppendLine("#assert aSystem deadlockfree;//used to check the number of states in initial model");
             return sb.ToString();

         }

         public static string LoadProbabilityModelPZMutualExlusion(int N)
         {
             StringBuilder sb = new StringBuilder();
             sb.AppendLine("//=======================Model Details===========================");
             sb.AppendLine("// mutual exclusion [PZ82]");
             sb.AppendLine("// dxp/gxn 19/12/99");
             sb.AppendLine("");
             sb.AppendLine("//*************below are some important guard conditions in the process(take n=4 for example). ");
             sb.AppendLine("");
             sb.AppendLine("//2,3 mean enter;");
             sb.AppendLine("//13,14 mean admit;");
             sb.AppendLine("//4,5,10,11,12 mean high;");
             sb.AppendLine("//6,9 mean tie;");
             sb.AppendLine("//7,8 mean low");
             sb.AppendLine("//10..14 (>9)mean Critical Section");
             sb.AppendLine("");
             sb.AppendLine("// atomic formula");
             sb.AppendLine("// none in low, high, tie");
             sb.AppendLine("//formula none_lht = !(p2>=4&p2<=12)&!(p3>=4&p3<=12)&!(p4>=4&p4<=12);");
             sb.AppendLine("// some in admit");
             sb.AppendLine("//formula some_a   = (p2>=13&p2<=14)|(p3>=13&p3<=14)|(p4>=13&p4<=14);");
             sb.AppendLine("// some in high, admit");
             sb.AppendLine("//formula some_ha  = (p2>=4&p2<=5|p2>=10&p2<=14)|(p3>=4&p3<=5|p3>=10&p3<=14)|(p4>=4&p4<=5|p4>=10&p4<=14);");
             sb.AppendLine("// none in high, tie, admit");
             sb.AppendLine("//formula none_hta = (p2>=0&p2<=3|p2>=7&p2<=8)&(p3>=0&p3<=3|p3>=7&p3<=8)&(p4>=0&p4<=3|p4>=7&p4<=8);");
             sb.AppendLine("// none in enter");
             sb.AppendLine("//formula none_e   = !(p2>=2&p2<=3)&!(p3>=2&p3<=3)&!(p4>=2&p4<=3);");
             sb.AppendLine("");
             sb.AppendLine("#define N " + N + ";");
             sb.AppendLine("");
             sb.AppendLine("var EnterCounter;");
             sb.AppendLine("var HighCounter;");
             sb.AppendLine("var LowCounter;");
             sb.AppendLine("var TieCounter;");
             sb.AppendLine("var AdmitCounter;");
             sb.AppendLine("var CriticalSectionCounter;");
             sb.AppendLine("");
             sb.AppendLine("");
             sb.AppendLine("Process0 = tau -> Process0 [] tau{EnterCounter++;} -> Process2;");
             sb.AppendLine("");
             sb.AppendLine("//Process1 = tau{EnterCounter++;} -> Process2;");
             sb.AppendLine("");
             sb.AppendLine("//have guard condition: (none_lht || some_a)");
             sb.AppendLine("Process2 = [(!(HighCounter>0||LowCounter>0||TieCounter>0)) || AdmitCounter>0] tau -> Process3;");
             sb.AppendLine("              ");
             sb.AppendLine("Process3 = tau{EnterCounter--; HighCounter++;} -> Process4 [] tau{EnterCounter--; LowCounter++} -> Process7;");
             sb.AppendLine("");
             sb.AppendLine("//have guard condition: some_ha");
             sb.AppendLine("Process4 = [HighCounter>1||AdmitCounter>0] tau -> Process5 [] [!(HighCounter>1||AdmitCounter>0)] enterCS{CriticalSectionCounter++;} -> Process10;");
             sb.AppendLine("             ");
             sb.AppendLine("Process5 = tau{TieCounter++; HighCounter--;} -> Process6;");
             sb.AppendLine("");
             sb.AppendLine("//have guard condition: !some_ha");
             sb.AppendLine("Process6 = [!(HighCounter>0||AdmitCounter>0)] tau -> Process9;");
             sb.AppendLine("              ");
             sb.AppendLine("//have guard condition: none_hta");
             sb.AppendLine("Process7 = [!(HighCounter>0||TieCounter>0||AdmitCounter>0)] tau -> Process8;");
             sb.AppendLine("             ");
             sb.AppendLine("Process8 = tau{LowCounter--; TieCounter++;} -> Process9;");
             sb.AppendLine("");
             sb.AppendLine("//have guard condition: none_lht");
             sb.AppendLine("Process10 = [!(LowCounter>0||HighCounter>1||TieCounter>0)] tau -> Process12 [] [(LowCounter>0||HighCounter>1||TieCounter>0)] tau -> Process11;");
             sb.AppendLine("              ");
             sb.AppendLine("Process11 = exitCS{HighCounter--; CriticalSectionCounter--;} -> Process0;");
             sb.AppendLine("");
             sb.AppendLine("Process12 = tau{AdmitCounter++; HighCounter--;} -> Process13;");
             sb.AppendLine("");
             sb.AppendLine("//have guard condition: none_e");
             sb.AppendLine("Process13 = [EnterCounter==0] tau -> Process14;");
             sb.AppendLine("");
             sb.AppendLine("Process14 = exitCS{AdmitCounter--; CriticalSectionCounter--;} -> Process0;");
             sb.AppendLine("");
             sb.AppendLine("Process9 = pcase{");
             sb.AppendLine("                    [0.5] : tau{TieCounter--; HighCounter++;} -> Process4");
             sb.AppendLine("                    default : tau{TieCounter--; LowCounter++;} -> Process7");
             sb.AppendLine("                   };");
             sb.AppendLine(" ");
             sb.AppendLine("aSpec = enterCS -> exitCS -> aSpec;");
             sb.AppendLine("");
             sb.AppendLine("System = ||| {N}@(Process0);");
             sb.AppendLine("#define safety (CriticalSectionCounter<=1);");
             sb.AppendLine("#define none_e (EnterCounter==0);");
             sb.AppendLine("#define all_high (HighCounter==N);");
             sb.AppendLine("#define all_low (LowCounter==N);");
             sb.AppendLine("");
             //sb.AppendLine("#assert System |= <>[]none_e with pmax;");
             //sb.AppendLine("#assert System |= []safety with pmin;");
             sb.AppendLine("#assert System refines aSpec with pmax;");
             //sb.AppendLine("#assert System refines aSpec with pmin;");

             sb.AppendLine("");
             sb.AppendLine("#assert System |= <>all_high && <>all_low with pmax;");
             sb.AppendLine("");
             sb.AppendLine("#assert System deadlockfree;//used to check the number of states in initial model");
             return sb.ToString();

         }



         public static string LoadProbabilityModelRabinMutualExlusion(int N)
         {
             int K =(int) (4 + Math.Ceiling(Math.Log(N, 2)));
             StringBuilder sb = new StringBuilder();
             sb.AppendLine("// N-processor mutual exclusion [Rab82]");
             sb.AppendLine("// gxn/dxp 03/12/08");
             sb.AppendLine("");
             sb.AppendLine("// to remove the need for fairness constraints for this model it is sufficent");
             sb.AppendLine("// to remove the self loops from the model ");
             sb.AppendLine("");
             sb.AppendLine("// the step corresponding to a process making a draw has been split into two steps");
             sb.AppendLine("// to allow us to identify states where a process will draw without knowing the value");
             sb.AppendLine("// randomly drawn");
             sb.AppendLine("// to correctly model the protocol and prevent erroneous behaviour, the two steps are atomic");
             sb.AppendLine("// (i.e. no other process can move one the first step has been made)");
             sb.AppendLine("// as for example otherwise an adversary can prevent the process from actually drawing");
             sb.AppendLine("// in the current round by not scheduling it after it has performed the first step");
             sb.AppendLine("");
             sb.AppendLine("");
             sb.AppendLine("// size of shared counter");
             sb.AppendLine("#define K "+K+"; // 4+ceil(log_2 N) in this case N = "+N);
             sb.AppendLine("");
             sb.AppendLine("// global variables (all modules can read and write)");
             sb.AppendLine("var c : {0..1}; // 0/1 critical section free/taken");
             sb.AppendLine("var B : {0..K}; // current highest draw");
             sb.AppendLine("var R : {1..2}; // current round");
             sb.AppendLine("var p[" + N + "] : {0..2}; // local state");
             sb.AppendLine("	//  0 remainder");
             sb.AppendLine("	//  1 trying");
             sb.AppendLine("	//  2 critical section");
             sb.AppendLine("var	b[" + N + "] : {0..K}; // current draw: bi");
             sb.AppendLine("var	r[" + N + "] : {0..2}; // current round: ri");
             sb.AppendLine("var	draw[" + N + "] : {0..1}; // performed first step of drawing phase");
             sb.AppendLine("");
             sb.AppendLine("// formula for process 1 drawing");
             sb.AppendLine("");
             sb.AppendLine("// formula to keep drawing phase atomic");
             sb.AppendLine("// (a process can only move if no other process is in the middle of drawing)");
             sb.AppendLine("");
             sb.AppendLine("process(i) =");
             sb.AppendLine("");
             sb.Append("	[");
             for(int i = 1; i<N; i++)
             {
                 sb.Append("draw[(i+"+i+")%"+N+"]==0 && ");
             }
             sb.AppendLine("p[i]==0]{p[i]=1;} -> process(i)");
             sb.AppendLine("	[]");
             sb.Append("	[");
             for (int i = 1; i < N; i++)
             {
                 sb.Append("draw[(i+" + i + ")%" + N + "]==0 && ");
             }
             sb.AppendLine("p[i]==1 && (B<b[i] || R!=r[i]) && draw[i]==0]{draw[i] = 1;} -> process(i)");
             sb.AppendLine("	[]");
             sb.Append("	[");
             for (int i = 1; i < N; i++)
             {
                 sb.Append("draw[(i+" + i + ")%" + N + "]==0 && ");
             }
             sb.AppendLine("p[i]==2] {p[i]=0; c=0;} -> process(i)");
             sb.AppendLine("	[]");
             sb.AppendLine("	[draw[i]==1]pcase{");
             for (int i = 1; i < K; i++ )
             {
                 sb.AppendLine("	         [" + (1.0 / Math.Pow(2, i)) + "] : [draw[i]==1] {b[i]= " + i + "; r[i]=R; draw[i]=0; if(B>" + i + "){B = B;}else{B = " + i + ";}} -> process(i)  ");
             }
             //sb.AppendLine("	         [0.25] : {b[i]=2; r[i]=R; draw[i]=0; if(B>2){B = B;}else{B = 2;}} -> process(i) ");
             //sb.AppendLine("	         [0.125] : {b[i]=3; r[i]=R; draw[i]=0; if(B>3){B = B;}else{B = 3;}} -> process(i) ");
             //sb.AppendLine("	         [0.0625] : {b[i]=4; r[i]=R; draw[i]=0; if(B>4){B = B;}else{B = 4;}} -> process(i) ");
             //sb.AppendLine("	         [0.03125] : [draw[i]==1]  {b[i]=5; r[i]=R; draw[i]=0; if(B>5){B = B;}else{B = 5;}} -> process(i) ");
             sb.AppendLine("	         default : [draw[i]==1] {b[i]=" + K + "; r[i]=R; draw[i]=0; if(B>" + K + "){B = B;}else{B = " + K + ";}} -> process(i) ");
             sb.AppendLine("	                 }");
             sb.AppendLine("	[]");
             sb.Append("	[");
             for (int i = 1; i < N; i++)
             {
                 sb.Append("draw[(i+" + i + ")%" + N + "]==0 && ");
             }
             sb.Append("p[i]==1 && B==b[i] && R==r[i] && c==0 ]");
             sb.AppendLine("pcase{");
             sb.Append("	         [0.5] :");
             sb.Append("	[");
             for (int i = 1; i < N; i++)
             {
                 sb.Append("draw[(i+" + i + ")%" + N + "]==0 && ");
             }
             sb.Append("p[i]==1 && B==b[i] && R==r[i] && c==0 ]");
             sb.AppendLine(" {R=1; c=1; B=0; b[i]=0; r[i]=0; p[i]=2;} -> process(i) ");
             sb.Append("	         default :");
             sb.Append("	[");
             for (int i = 1; i < N; i++)
             {
                 sb.Append("draw[(i+" + i + ")%" + N + "]==0 && ");
             }
             sb.Append("p[i]==1 && B==b[i] && R==r[i] && c==0 ]");
             sb.AppendLine(" {R=2; c=1; B=0; b[i]=0; r[i]=0; p[i]=2;} -> process(i) ");
             sb.AppendLine("	     }");
             sb.AppendLine("	;");
             sb.AppendLine("System = ||| i:{0.."+(N-1)+"}@(process(i));");
             sb.AppendLine("	     ");
             sb.Append("#define one_critical (");
             for(int i = 0; i<N-1; i++)
             {
                 sb.Append("p[" + i + "]==2||");
             }
             sb.AppendLine("p[" + (N - 1) + "]==2);");
             sb.Append("#define one_trying (");
             for (int i = 0; i < N - 1; i++)
             {
                 sb.Append("p[" + i + "]==1||");
             }
             
             sb.AppendLine("p[" + (N - 1) + "]==1);");
             sb.Append("#define safety (");
             for (int i = 0; i < N - 1; i++)
             {
                 for (int j = 1; j < N - 1; j++)
                 {
                     sb.Append("p[" + (i + j) % N + "]<2&&");
                 }
                 sb.Append("p[" + (i + N - 1) % N + "]<2||");
             }
             for (int j = 1; j < N - 1; j++)
             {
                 sb.Append("p[" + (N - 1 + j) % N + "]<2&&");
             }
             sb.AppendLine("p[" + (2 * N - 2) % N + "]<2);");
             sb.AppendLine("#assert System |= ([] safety) with pmax;");
             sb.AppendLine("#assert System |= ([] safety) with pmin;");
             sb.AppendLine("#assert System |= (<> one_critical) with pmax;//liveness; negation is safety ");
             sb.AppendLine("#assert System |= (<> one_critical) with pmin;//liveness; negation is safety ");
             sb.AppendLine("#assert System |= ([]!one_critical) with pmax;//safety. so fast. ");
             sb.AppendLine("#assert System |= ([]!one_critical) with pmin;//safety so fast. ");
             sb.AppendLine("");
             sb.AppendLine("#assert System deadlockfree;//used to check the number of states in initial model");
             return sb.ToString();
         }

         public static string LoadProbabilityModelCSMACD(int N, int K)
         {
             StringBuilder sb = new StringBuilder();
             sb.AppendLine("// CSMA/CD protocol - probabilistic version of kronos model (3 stations)");
             sb.AppendLine("// gxn/dxp 04/12/01");
             sb.AppendLine("");
             sb.AppendLine("#define N " + N +";// number of processes");
             sb.AppendLine("#define K " + K + ";// exponential backoff limit");
             sb.AppendLine("#define slot 2;// length of slot");
             sb.AppendLine("#define sigma 1;// time for messages to propagate along the bus");
             sb.AppendLine("#define lambda 30;// time to send a message");
             sb.AppendLine("#define M " + (int)(Math.Floor(Math.Pow(2, K)) - 1) + ";// max number of slots to wait");
             sb.AppendLine("//bus situation");
             sb.AppendLine("#define idle 0;");
             sb.AppendLine("#define active 1;");
             sb.AppendLine("#define collision 2;");
             sb.AppendLine("");
             sb.AppendLine("var bus = idle;");
             sb.AppendLine("var state["+N+"];///represent 2 processes");
             sb.AppendLine("//clock of bus");
             sb.AppendLine("var y1 : {0..sigma+1}=0;// time since first send (used find time until channel sensed busy)");
             sb.AppendLine("var y2 : {0..sigma+1}=0;// time since second send (used to find time until collision detected)");
             sb.AppendLine("var x[" + N + "] : {0..30};// LOCAL CLOCK");
             sb.AppendLine("");
             sb.AppendLine("var cd[" + N + "]:{0..K};// BACKOFF COUNTER (number of slots to wait)");
             sb.AppendLine("var bc[" + N + "]:{0..M};// COLLISION COUNTER");
             sb.AppendLine("");
             sb.AppendLine("bus_situation(i) =");
             sb.AppendLine("               // a sender sends (ok - no other message being sent)");
             sb.AppendLine("               [bus == idle]sending.i{bus = active;}->bus_situation(i)");
             sb.AppendLine("               []");
             sb.AppendLine("               // a sender sends (bus busy - collision)");
             sb.AppendLine("               [y1<1&&(bus == active||bus == collision)]sending.i{bus = collision;}->bus_situation(i)");
             sb.AppendLine("               []");
             sb.AppendLine("               // finish sending");
             sb.AppendLine("               [bus == active]end.i{bus = idle; y1 = 0}->bus_situation(i)");
             sb.AppendLine("               []");
             sb.AppendLine("               // bus busy");
             sb.AppendLine("               [y1>=1&&(bus == active||bus == collision)]busy.i{bus = bus}->bus_situation(i)");
             sb.AppendLine("               []");
             sb.AppendLine("               // collision detected");
             sb.AppendLine("               [bus == collision&&y2<=1]cdetect{bus = idle; y1 = 0; y2 = 0}->bus_situation(i)");
             sb.AppendLine("               []");
             sb.AppendLine("               // time passage");
             sb.AppendLine("               [bus == idle]time{y1 = 0;}->bus_situation(i)// value of y1/y2 does not matter in state 0");
             sb.AppendLine("               []");
             sb.AppendLine("               [bus == active]time{if(y1>1){y1 = sigma+1;}else{y1 = y1+1;}}->bus_situation(i) // no invariant in state 1");
             sb.AppendLine("               []");
             sb.AppendLine("               [bus == collision&&y2<1]time{if(y1>1){y1 = sigma+1;}else{y1 = y1+1;}if(y2>1){y2 = sigma+1;}else{y2 = y2+1;}}->bus_situation(i); // invariant in state 2 (time until collision detected)");
             sb.AppendLine("	");
             sb.AppendLine(" ");
             sb.AppendLine("#define initial 0;");
             sb.AppendLine("#define transmit 1;");
             sb.AppendLine("#define Scollision 2;");
             sb.AppendLine("#define wait 3;");
             sb.AppendLine("#define sent 4;");
             sb.AppendLine("#define unusual 5;");
             sb.AppendLine("");
             sb.AppendLine("station(i) =");
             sb.AppendLine("               // start sending");
             sb.AppendLine("               [state[i] == initial]sending.i{state[i] = transmit; x[i] = 0;}->station(i)");
             sb.AppendLine("               []");
             sb.AppendLine("               [state[i] == initial]busy.i{state[i] = Scollision; x[i] = 0; if(K>cd[i]+1){cd[i]=cd[i]+1}else{cd[i]=K}}->station(i)");
             sb.AppendLine("               []");
             sb.AppendLine("               // transmitting");
             sb.AppendLine("               [state[i] == transmit&&x[i]<lambda]time{if(x[i]+1>lambda){x[i]=lambda;}else{x[i]=x[i]+1}}->station(i)// let time pass");
             sb.AppendLine("               []");
             sb.AppendLine("               [state[i] == transmit&&x[i] == lambda]end.i{state[i]=sent; x[i]=0;}->station(i)// finished");
             sb.AppendLine("               []");
             sb.AppendLine("               [state[i] == transmit]cdetect{state[i] = Scollision; x[i] = 0; if(K>cd[i]+1){cd[i]=cd[i]+1}else{cd[i]=K}}->station(i)// collision detected (increment backoff counter)");
             sb.AppendLine("               []");
             sb.AppendLine("               [state[i] != transmit]cdetect{state[i]=state[i]}->station(i)// add loop for collision detection when not important");
             sb.AppendLine("               []");
             sb.AppendLine("               // wait until backoff counter reaches 0 then send again");
             sb.AppendLine("               [state[i] == wait&&x[i]<slot]time{x[i] = x[i]+1;}-> station(i)");
             sb.AppendLine("               []");
             sb.AppendLine("               [state[i] == wait&&x[i] == slot&&bc[i]>0]time{x[i] = 1; bc[i] = bc[i]-1}->station(i)");
             sb.AppendLine("               []");
             sb.AppendLine("               [state[i] == wait&&x[i] == slot&&bc[i] == 0]sending.i{state[i] = transmit; x[i] = 0;}->station(i)");
             sb.AppendLine("               []");
             sb.AppendLine("               [state[i] == wait&&x[i] == slot&&bc[i] == 0]busy.i{state[i] = Scollision; x[i] = 0; if(K>cd[i]+1){cd[i]=cd[i]+1}else{cd[i]=K}}->station(i)");
             sb.AppendLine("               []");
             sb.AppendLine("               // once finished nothing matters");
             sb.AppendLine("               [state[i] == sent||state[i] == unusual]time{x[i] = 0;}->station(i)");
             sb.AppendLine("               []");
             sb.AppendLine("               // set backoff (no time can pass in this state)");
             sb.AppendLine("            	// probability depends on which transmission this is (cd1)");
             sb.AppendLine("               [state[i]==Scollision&&cd[i]==1]pcase");
             sb.AppendLine("               {");
             sb.AppendLine("                      [0.5] : [state[i]==Scollision&&cd[i]==1]{state[i]=wait;bc[i]=0;} -> station(i)");
             sb.AppendLine("                      default : [state[i]==Scollision&&cd[i]==1]{state[i]=wait;bc[i]=1;} -> station(i)");
             sb.AppendLine("               }");
             sb.AppendLine("               []");
             sb.AppendLine("               [state[i]==Scollision&&cd[i]==2]pcase");
             sb.AppendLine("               {");
             sb.AppendLine("                      [0.25] : [state[i]==Scollision&&cd[i]==2]{state[i]=wait;bc[i]=0;} -> station(i)");
             sb.AppendLine("                      [0.25] : [state[i]==Scollision&&cd[i]==2]{state[i]=wait;bc[i]=1;} -> station(i)");
             sb.AppendLine("                      [0.25] : [state[i]==Scollision&&cd[i]==2]{state[i]=wait;bc[i]=2;} -> station(i)");
             sb.AppendLine("                      default :[state[i]==Scollision&&cd[i]==2]{state[i]=wait;bc[i]=3;} -> station(i)");
             sb.AppendLine("               }");
             sb.AppendLine("               ;");
             sb.AppendLine("");
             sb.AppendLine("System() = || i:{0..N-1} @(bus_situation(i)||station(i)); ");
             sb.AppendLine("");
             sb.Append("#define all_delivered (");
             for(int i=0; i<N-1;i++)
             {
                 sb.Append("state[" + i + "]==sent&&");
             }
             sb.AppendLine("state[" + (N - 1) + "]==sent);");
             sb.Append("#define one_delivered (");
             for(int i=0; i<N-1;i++)
             {
                 sb.Append("state[" + i + "]==sent||");
             }
             sb.AppendLine("state[" + (N - 1) + "]==sent);");
             sb.Append("#define collision_max_backoff (");
             for (int i = 0; i < N - 1; i++)
             {
                 sb.Append("cd[" + i + "]==K && state[" + i + "]==transmit && bus==collision||");
             }
             sb.AppendLine("cd[" + (N - 1) + "]==K && state[" + (N - 1) + "]==transmit && bus==collision);");
             //sb.AppendLine("#define collision_max_backoff ((cd[0]==K && state[0]==transmit && bus==collision)||(cd[1]==K && state[1]==transmit && bus==collision));");
             sb.AppendLine("");
             sb.AppendLine("#assert System |= (!collision_max_backoff U all_delivered) with pmin;// probability[0,0.1];");
             sb.AppendLine("#assert System |= !(!collision_max_backoff U all_delivered) with pmax;//negation of the above one. and is a safety");
             sb.AppendLine("");
             sb.AppendLine("");
             sb.AppendLine("//********** can not get the result");
             sb.AppendLine("");
             sb.AppendLine("#assert System deadlockfree;//used to check the number of states in initial model");
             return sb.ToString();
         }
         public static string LoadProbConsensus(int N, int K)
         {
             StringBuilder sb = new StringBuilder();
             sb.AppendLine("// COIN FLIPPING PROTOCOL FOR POLYNOMIAL RANDOMIZED CONSENSUS [AH90] ");
             sb.AppendLine("// gxn/dxp 20/11/00");
             sb.AppendLine("");
             sb.AppendLine("");
             sb.AppendLine("// constants");
             sb.AppendLine("#define N " + N + ";");
             sb.AppendLine("#define K " + K + ";");
             sb.AppendLine("#define range " + (2*(K + 1)*N) + ";//(2*(K+1)*N);");
             sb.AppendLine("#define counter_init " + ((K + 1)*N) + ";//((K+1)*N);");
             sb.AppendLine("#define left " + N + ";//N;");
             sb.AppendLine("#define right " + (2*(K + 1)*N - N) + ";//(2*(K+1)*N - N);");
             sb.AppendLine("");
             sb.AppendLine("var counter : {0..range}=counter_init;// shared coin");
             sb.AppendLine("var Pcounter; //used to record the process number");
             sb.AppendLine("var coin0counter = N;//used to record the number of coins which are 0;");
             sb.AppendLine("var coin1counter;//used to record the number of coins which are 1;");
             sb.AppendLine("");
             sb.AppendLine("//local variable");
             sb.AppendLine("// 0 - flip");
             sb.AppendLine("// 1 - write ");
             sb.AppendLine("// 2 - check");
             sb.AppendLine("// 3 - finished");
             sb.AppendLine("// processij means this process's coin is i and its local variable is j");
             sb.AppendLine("");
             sb.AppendLine("process00 = pcase{");
             sb.AppendLine("                 [0.5] : process01");
             sb.AppendLine("                 default : tau {coin0counter--; coin1counter++;} -> process11");
             sb.AppendLine("                 };");
             sb.AppendLine("                ");
             sb.AppendLine("process01 = [counter>0] tau {counter--;} -> process02;");
             sb.AppendLine("");
             sb.AppendLine("process11 = [counter<range] tau {counter++; coin1counter--; coin0counter++;} -> process02;");
             sb.AppendLine("");
             sb.AppendLine("process02 = [(counter<=left)] tau {Pcounter++;} -> process03");
             sb.AppendLine("            []");
             sb.AppendLine("            [(counter>=right)] tau {Pcounter++; coin0counter--; coin1counter++;} -> process13");
             sb.AppendLine("            []");
             sb.AppendLine("            [(counter>left) && (counter<right)] tau -> process00;");
             sb.AppendLine("");
             sb.AppendLine("process03 = [Pcounter==N] done -> process03;");
             sb.AppendLine("");
             sb.AppendLine("process13 = [Pcounter==N] done -> process13;");
             sb.AppendLine("");
             sb.AppendLine("SpecProcess= done -> SpecProcess;");
             sb.AppendLine("");
             sb.AppendLine("System = |||{N}@ process00;");
             sb.AppendLine("");
             sb.AppendLine("// labels");
             sb.AppendLine("#define finished  (Pcounter==N);");
             sb.AppendLine("#define all_coins_equal_0 (coin0counter==N);");
             sb.AppendLine("#define all_coins_equal_1 (coin1counter==N);");
             sb.AppendLine("#define agree (coin0counter==N || coin1counter==N);");
             //sb.AppendLine("");
             //sb.AppendLine("#assert System |= <>finished with pmax;");
             //sb.AppendLine("#assert System |= <> !finished;");
             //sb.AppendLine("#assert System |= []!finished;");
             //sb.AppendLine("#assert System |= []finished;");
             //sb.AppendLine("#assert System |= [](finished->[]finished);");
             sb.AppendLine("#assert System |= <>all_coins_equal_0 && <>all_coins_equal_1 with pmax;");
             sb.AppendLine("");
             //sb.AppendLine("#assert System |= []<>all_coins_equal_0 with pmax;");
             //sb.AppendLine("#assert System |= []<>all_coins_equal_0 with pmin;");
             //sb.AppendLine("#assert System |= <>(finished&&all_coins_equal_0)with pmax;");
             //sb.AppendLine("#assert System |= <>(finished&&all_coins_equal_0)with pmin;");
             //sb.AppendLine("");
             sb.AppendLine("#assert System refines SpecProcess;");
             sb.AppendLine("");
             sb.AppendLine("#assert System deadlockfree;//used to check the number of states in initial model");

             return sb.ToString();
         }
        public static string LoadProbDinningPhil(int N, bool fair)
        {

            StringBuilder sb = new StringBuilder();

            if (fair == false)
            {
                sb.AppendLine("//=======================Model Details===========================");
                sb.AppendLine("// randomized dining philosophers [LR81] no fair");
                sb.AppendLine("// dxp/gxn 23/01/02");
                sb.AppendLine("");
                sb.AppendLine("// model which does not require fairness");
                sb.AppendLine("// remove the possibility of loops:");
                sb.AppendLine("// (1) cannot stay in thinking");
                sb.AppendLine("// (2) if first fork not free then cannot move (another philosopher must more)");
                sb.AppendLine("");
                sb.AppendLine("//mdp");
                sb.AppendLine("");
                sb.AppendLine("#define N " + N + ";");
                sb.AppendLine("");
                sb.AppendLine("var Fork[N];      //0 means free and 1 means not free");
                sb.AppendLine("var hungryCounter;");
                sb.AppendLine("var eatCounter;");
                sb.AppendLine("");
                sb.AppendLine("//trying");
                sb.AppendLine("phil0(i) = Hungry{hungryCounter++;} -> phil1(i);");
                sb.AppendLine("");
                sb.AppendLine("// draw randomly");
                sb.AppendLine("phil1(i) = pcase{");
                sb.AppendLine("               [0.5] : [Fork[(i+1)%N]==0] tau {Fork[(i+1)%N]=1;}-> phil2(i)");
                sb.AppendLine("               default : [Fork[(i+N-1)%N]==0] tau {Fork[(i+N-1)%N]=1;} -> phil3(i)");
                sb.AppendLine("               };");
                sb.AppendLine("");
                sb.AppendLine("          // pick up right (got left)");
                sb.AppendLine("phil2(i) = [Fork[(i+N-1)%N]==0] Eat {Fork[(i+N-1)%N]=1; hungryCounter--; eatCounter++;} -> phil4(i)");
                sb.AppendLine("          []");
                sb.AppendLine("          // right not free (got left)");
                sb.AppendLine("          [Fork[(i+N-1)%N]==1] tau {Fork[(i+1)%N]=0;}-> phil1(i);");
                sb.AppendLine("");
                sb.AppendLine("          // pick up left (got right)");
                sb.AppendLine("phil3(i) = [Fork[(i+1)%N]==0] Eat {Fork[(i+1)%N]=1; hungryCounter--; eatCounter++;} -> phil4(i)");
                sb.AppendLine("          []");
                sb.AppendLine("          // left not free (got right)");
                sb.AppendLine("          [Fork[(i+1)%N]==1] tau {Fork[(i+N-1)%N]=0;}-> phil1(i);");
                sb.AppendLine("");
                sb.AppendLine("// finished eating and put down left[]right");
                sb.AppendLine("phil4(i) = tau{Fork[(i+1)%N]=0; eatCounter--;} -> tau{Fork[(i+N-1)%N]=0;} -> phil0(i) ");
                sb.AppendLine("           []");
                sb.AppendLine("           tau{Fork[(i+N-1)%N]=0; eatCounter--;} -> tau{Fork[(i+1)%N]=0;} -> phil0(i);");
                sb.AppendLine("");
                sb.AppendLine("System = |||i:{0..N-1}@phil0(i);");
                sb.AppendLine("");
                sb.AppendLine("SpecProcess = Hungry -> SpecProcess [] Eat -> NotHungry;");
                sb.AppendLine("");
                sb.AppendLine("NotHungry = Eat -> NotHungry;");
                sb.AppendLine("");
                sb.AppendLine("#define hungry (hungryCounter>0);");
                sb.AppendLine("#define eat_one (eatCounter>0);");
                sb.AppendLine("#define eat_all (eatCounter==N);");
                //sb.AppendLine("#assert System |= <>eat_all with pmax;");
                //sb.AppendLine("#assert System |= <>[]!hungry with pmax;");
                sb.AppendLine("#assert System |= [](eat_one->[]!hungry) with pmax;");
                sb.AppendLine("#assert System |= <>eat_one && <>hungry with pmax;");
                sb.AppendLine("");
                sb.AppendLine("#assert System deadlockfree;//used to check the number of states in initial model");
                sb.AppendLine("");
                sb.AppendLine("#assert System refines SpecProcess with pmax;");
            }
            else
            {
                sb.AppendLine("// randomized dining philosophers [LR81] has fairness");
                sb.AppendLine("// dxp/gxn 23/01/02");
                sb.AppendLine("");
                sb.AppendLine("");
                sb.AppendLine("//mdp");
                sb.AppendLine("");
                sb.AppendLine("#define N " + N + ";");
                sb.AppendLine("");
                sb.AppendLine("var Fork[N];      //0 means free and 1 means not free");
                sb.AppendLine("var hungryCounter;");
                sb.AppendLine("var eatCounter;");
                sb.AppendLine("");
                sb.AppendLine("//trying                                           or keep thinking");
                sb.AppendLine("phil0(i) = Hungry{hungryCounter++;} -> phil1(i) [] tau -> phil0(i);");
                sb.AppendLine("");
                sb.AppendLine("// draw randomly");
                sb.AppendLine("phil1(i) = pcase{       ");
                sb.AppendLine("                [0.5] : phil2(i)");
                sb.AppendLine("                default : phil3(i)");
                sb.AppendLine("                };");
                sb.AppendLine("                ");
                sb.AppendLine("// pick up left if left fork is free   ");
                sb.AppendLine("phil2(i) = [Fork[(i+1)%N]==0]tau{Fork[(i+1)%N]=1;} -> phil4(i) [] [Fork[(i+1)%N]==1]tau -> phil2(i);");
                sb.AppendLine("");
                sb.AppendLine("// pick up right if right fork is free ");
                sb.AppendLine("phil3(i) = [Fork[(i+N-1)%N]==0]tau{Fork[(i+N-1)%N]=1;} -> phil5(i) [] [Fork[(i+N-1)%N]==1]tau -> phil3(i);");
                sb.AppendLine("");
                sb.AppendLine("           // pick up right (got left)");
                sb.AppendLine("phil4(i) = [Fork[(i+N-1)%N]==0] Eat {Fork[(i+N-1)%N]=1; hungryCounter--; eatCounter++;} -> phil6(i)");
                sb.AppendLine("           []");
                sb.AppendLine("           // right not free (got left)");
                sb.AppendLine("           [Fork[(i+N-1)%N]==1] tau {Fork[(i+1)%N]=0;}-> phil1(i);");
                sb.AppendLine("           ");
                sb.AppendLine("           // pick up left (got right)");
                sb.AppendLine("phil5(i) = [Fork[(i+1)%N]==0] Eat {Fork[(i+1)%N]=1; hungryCounter--; eatCounter++;} -> phil6(i)");
                sb.AppendLine("           []");
                sb.AppendLine("           // left not free (got right)");
                sb.AppendLine("           [Fork[(i+1)%N]==1] tau {Fork[(i+N-1)%N]=0;}-> phil1(i);");
                sb.AppendLine("");
                sb.AppendLine("// finished eating and put down left[]right");
                sb.AppendLine("");
                sb.AppendLine("phil6(i) = tau{Fork[(i+1)%N]=0; eatCounter--;} -> tau{Fork[(i+N-1)%N]=0;} -> phil0(i) ");
                sb.AppendLine("           []");
                sb.AppendLine("           tau{Fork[(i+N-1)%N]=0; eatCounter--;} -> tau{Fork[(i+1)%N]=0;} -> phil0(i);");
                sb.AppendLine("");
                sb.AppendLine("System = |||i:{0..N-1}@phil0(i);");

                sb.AppendLine("");
                sb.AppendLine("SpecProcess = Hungry -> SpecProcess [] Eat -> NotHungry;");
                sb.AppendLine("");
                sb.AppendLine("NotHungry = Eat -> NotHungry;");
                sb.AppendLine("");
                sb.AppendLine("#define hungry (hungryCounter>0);");
                sb.AppendLine("#define eat_one (eatCounter>0);");
                sb.AppendLine("#define eat_all (eatCounter==N);");
                //sb.AppendLine("#assert System |= <>eat_all with pmax;");
                //sb.AppendLine("#assert System |= <>[]!hungry with pmax;");
                sb.AppendLine("#assert System |= [](eat_one->[]!hungry) with pmax;");
                sb.AppendLine("#assert System |= <>eat_one && <>hungry with pmax;");
                sb.AppendLine("");
                sb.AppendLine("#assert System deadlockfree;//used to check the number of states in initial model");
                sb.AppendLine("");
                sb.AppendLine("#assert System refines SpecProcess with pmax;");
            }

            return sb.ToString();
        }

        public static string LoadDBMTesting(int N, int K)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("#import \"PAT.Lib.DBM\";");
            sb.AppendLine("#import \"PAT.Lib.Set\";");
            sb.AppendLine("#import \"PAT.Math\";");
            sb.AppendLine("");
            sb.AppendLine("//DBM model has two DBMs: dbm1 and dbm2");
            sb.AppendLine("#define N " + N + ";//the number of clocks of each DBM");
            sb.AppendLine("#define Ceiling " + K + ";//the number of constraints added in each DBM");
            sb.AppendLine("#define Bound " + (N*K) + "; //the timer bound");
            sb.AppendLine("");
            sb.AppendLine("var<DBM> dbm1 = new DBM(Ceiling);");
            sb.AppendLine("var<Set> timers1 = new Set();");
            sb.AppendLine("var timerCount1 = 0;");
            sb.AppendLine("var timerID1 = 1;");
            sb.AppendLine("var result1 = false;");
            sb.AppendLine("var isBounded = true;");
            sb.AppendLine("var containsClock1 = false;");
            sb.AppendLine("var stopped = false;");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("DBMTest1() = ifa(timerCount1 < N) ");
            sb.AppendLine("			 {");
            sb.AppendLine("				newTimerID{timerID1 = dbm1.GetNewTimerID();stopped=false} -> AddTimer.timerID1{dbm1.AddTimer(timerID1); timers1.Add(timerID1); timerCount1 = timers1.Count();} -> ");
            sb.AppendLine("				(Delay{dbm1.Delay()} -> 			");
            sb.AppendLine("				(OneCycle1; ");
            sb.AppendLine("					 check{result1 = dbm1.IsConstraintNotSatisfied(); isBounded=dbm1.IsTimersBounded(Bound);} ->");
            sb.AppendLine("					 ifa (!result1)");
            sb.AppendLine("					 {");
            sb.AppendLine("					 	ConstraintSatisfied -> (KeepTProcess1(timerCount1, call(pow, 2, timerCount1)); (DBMTest1() [] ResetProcess1(timerCount1)))");
            sb.AppendLine("					 }");
            sb.AppendLine("					 else ");
            sb.AppendLine("					 {");
            sb.AppendLine("					 	ConstraintNotSatisfied{dbm1 = new DBM(Ceiling); timers1 = new Set(); timerCount1 =0; timerID1 = 1;} -> DBMTest1()");
            sb.AppendLine("					 }");
            sb.AppendLine("					)");
            sb.AppendLine("				)");
            sb.AppendLine("			 }");
            sb.AppendLine("			 else");
            sb.AppendLine("			 {");
            sb.AppendLine("				stop{stopped =true;} -> DBMTest1()");
            sb.AppendLine("			 };");
            sb.AppendLine("");
            sb.AppendLine("OneCycle1() = (AddCProcess1(timerCount1); OneCycle1)");
            sb.AppendLine("		      [] (Clone{dbm1.Clone()} -> OneCycle1)");
            sb.AppendLine("	    	  [] Skip;");
            sb.AppendLine("			");
            sb.AppendLine("ResetProcess1(size) = ifa(size > 0) { ");
            sb.AppendLine("					 	[]t:{0..size-1}@ResetTimer{dbm1.ResetTimer(timers1.Get(t))} -> DBMTest1()");
            sb.AppendLine("					  } else {");
            sb.AppendLine("					 	 DBMTest1()");
            sb.AppendLine("					  };");
            sb.AppendLine("			");
            sb.AppendLine("KeepTProcess1(size, powset) =  ifa(size > 0) ");
            sb.AppendLine("					 		   {");
            sb.AppendLine("						 			ifa(powset ==1)");
            sb.AppendLine("						 			{");
            sb.AppendLine("						 				KeepTimers.1{dbm1 = dbm1.KeepTimers(timers1.GetSubsetByIndex(1)); timerCount1 = timers1.Count(); containsClock1=timers1.Contains(1); } -> Skip");
            sb.AppendLine("						 			}");
            sb.AppendLine("						 	        else");
            sb.AppendLine("						 			{");
            sb.AppendLine("					 					[] t:{1..powset-1}@KeepTimers.t{dbm1 = dbm1.KeepTimers(timers1.GetSubsetByIndex(t)); timerCount1 = timers1.Count(); containsClock1=timers1.Contains(1);} -> Skip");
            sb.AppendLine("						 			}");
            sb.AppendLine("					 		   };					");
            sb.AppendLine("");
            sb.AppendLine("AddCProcess1(size) = ifa(size > 0) {");
            sb.AppendLine("						([] t:{0..size-1}@ ([]op:{0..2}@ ([]value:{0..Ceiling}@ AddConstraint.t.op.value{dbm1.AddConstraint(timers1.Get(t), op, value);} -> Skip )))");
            sb.AppendLine("					 };	 ");
            sb.AppendLine("					");
            sb.AppendLine("					");
            sb.AppendLine("#assert DBMTest1 deadlockfree;");
            sb.AppendLine("");
            sb.AppendLine("#define goal timerID1 == 0;");
            sb.AppendLine("#assert DBMTest1 reaches goal;");
            sb.AppendLine("");
            sb.AppendLine("#define goal1 isBounded == true;");
            sb.AppendLine("#assert DBMTest1 |= []goal1;");
            sb.AppendLine("");
            sb.AppendLine("#define goal2 stopped==true || containsClock1 == false;");
            sb.AppendLine("#assert DBMTest1 |= []<>goal2;	");
            sb.AppendLine("	");
            sb.AppendLine("//==============================================================================");
            sb.AppendLine("var<DBM> dbm2 = new DBM(Ceiling);");
            sb.AppendLine("var<Set> timers2 = new Set();");
            sb.AppendLine("var timerCount2 = 0;");
            sb.AppendLine("var timerID2 = 1;");
            sb.AppendLine("var result2 = false;");
            sb.AppendLine("var isBounded2 = true;");
            sb.AppendLine("var containsClock2 = false;");
            sb.AppendLine("var stopped2 = false;");
            sb.AppendLine("");
            sb.AppendLine("DBMTest2() = ifa(timerCount2 < N)");
            sb.AppendLine("			{");
            sb.AppendLine("				newTimerID{timerID2 = dbm2.GetNewTimerID();stopped2=false} -> AddTimer.timerID2{dbm2.AddTimer(timerID2); timers2.Add(timerID2); timerCount2 = timers2.Count();} -> ");
            sb.AppendLine("				(Delay{dbm2.Delay()} -> 			");
            sb.AppendLine("				(OneCycle2; ");
            sb.AppendLine("					 check{result2 = dbm2.IsConstraintNotSatisfied(); isBounded2=dbm2.IsTimersBounded(Bound);} ->");
            sb.AppendLine("					 ifa (!result2)");
            sb.AppendLine("					 {");
            sb.AppendLine("					 	ConstraintSatisfied -> (KeepTProcess2(timerCount2, call(pow, 2, timerCount2)); (DBMTest2() [] ResetProcess2(timerCount2)))");
            sb.AppendLine("					 }");
            sb.AppendLine("					 else ");
            sb.AppendLine("					 {");
            sb.AppendLine("					 	ConstraintNotSatisfied{dbm2 = new DBM(Ceiling); timers2 = new Set(); timerCount2 =0; timerID2 = 1;} -> DBMTest2()");
            sb.AppendLine("					 }");
            sb.AppendLine("					)");
            sb.AppendLine("				)");
            sb.AppendLine("			}");
            sb.AppendLine("			else");
            sb.AppendLine("			{");
            sb.AppendLine("				stop{stopped2 =true;} -> DBMTest2()");
            sb.AppendLine("			}");
            sb.AppendLine("			;");
            sb.AppendLine("");
            sb.AppendLine("OneCycle2 = (AddCProcess2(timerCount2); OneCycle2)");
            sb.AppendLine("		[] (Clone{dbm2.Clone()} -> OneCycle2)");
            sb.AppendLine("		[] Skip;");
            sb.AppendLine("			");
            sb.AppendLine("ResetProcess2(size) = ifa(size > 0) { ");
            sb.AppendLine("					 	[]t:{0..size-1}@ResetTimer{dbm2.ResetTimer(timers2.Get(t))} -> DBMTest2()");
            sb.AppendLine("					 } else {");
            sb.AppendLine("					 	DBMTest2()");
            sb.AppendLine("					 };");
            sb.AppendLine("			");
            sb.AppendLine("KeepTProcess2(size, powset) = ifa(size > 0) ");
            sb.AppendLine("					 		  {");
            sb.AppendLine("						 			ifa(powset ==1)");
            sb.AppendLine("						 			{");
            sb.AppendLine("						 				KeepTimers.1{dbm2 = dbm2.KeepTimers(timers2.GetSubsetByIndex(1)); timerCount2 = timers2.Count(); containsClock2=timers2.Contains(1); } -> Skip");
            sb.AppendLine("						 			}");
            sb.AppendLine("						 	        else");
            sb.AppendLine("						 			{");
            sb.AppendLine("					 					[] t:{1..powset-1}@KeepTimers.t{dbm2 = dbm2.KeepTimers(timers2.GetSubsetByIndex(t)); timerCount2 = timers2.Count(); containsClock2=timers2.Contains(1);} -> Skip");
            sb.AppendLine("						 			}");
            sb.AppendLine("					 		  };					");
            sb.AppendLine("");
            sb.AppendLine("AddCProcess2(size) = ifa(size > 0) {");
            sb.AppendLine("						([] t:{0..size-1}@ ([]op:{0..2}@ ([]value:{0..Ceiling}@ AddConstraint.t.op.value{dbm2.AddConstraint(timers2.Get(t), op, value);} -> Skip )))");
            sb.AppendLine("					 };	 ");
            sb.AppendLine("");
            sb.AppendLine("//==============================================================================");
            sb.AppendLine("aDBMTest = DBMTest1 ||| DBMTest2;");
            sb.AppendLine("");
            sb.AppendLine("#assert aDBMTest deadlockfree;");
            sb.AppendLine("");
            sb.AppendLine("#define goal3 timerID1 == 0;");
            sb.AppendLine("#assert aDBMTest reaches goal3;");
            sb.AppendLine("");
            sb.AppendLine("#define goal4 isBounded == true && isBounded2 == true;");
            sb.AppendLine("#assert aDBMTest |= []goal4;");
            sb.AppendLine("");
            sb.AppendLine("#define goal5 (stopped==true || containsClock1 == false) && (stopped2 ==true || containsClock2 == false);");
            sb.AppendLine("#assert aDBMTest |= []<>goal5;");
            return sb.ToString();
        }
    }
}