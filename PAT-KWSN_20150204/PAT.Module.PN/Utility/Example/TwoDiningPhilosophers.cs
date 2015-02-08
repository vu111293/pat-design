using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PAT.PN.Utility.Example
{
    class TwoDiningPhilosophers
    {
        public static string TwoDiningPhilosophersProblem()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<PN>");
            sb.AppendLine(" <Declaration>");
            sb.AppendLine("     System = Phils();");
            sb.AppendLine("     #assert System deadlockfree;");
            sb.AppendLine("     #assert System |= []&lt;&gt; eat_0;");
            sb.AppendLine("     #assert System |= []&lt;&gt; eat_1;");
            sb.AppendLine(" </Declaration>");
            sb.AppendLine(" <Models>");
            sb.AppendLine("     <Model Name=\"Phils\" Parameter=\"\" Zoom=\"1\" PlaceCounter=\"9\" TransitionCounter=\"1\">");
            sb.AppendLine("         <Places>");
            sb.AppendLine("             <Place Name=\"has_left_0\" NumOfToken=\"0\" Capacity=\"0\">");
            sb.AppendLine("                 <Position X=\"6.8\" Y=\"3.7\" Width=\"0.2\" />");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"6.8\" Y=\"4\" Width=\"0.7\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("                 <Guard></Guard>");
            sb.AppendLine("             </Place>");

            sb.AppendLine("             <Place Name=\"has_right_0\" NumOfToken=\"0\" Capacity=\"0\">");
            sb.AppendLine("                 <Position X=\"6.8\" Y=\"1.7\" Width=\"0.2\" />");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"6.6\" Y=\"1.4\" Width=\"0.8\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("                 <Guard></Guard>");
            sb.AppendLine("             </Place>");

            sb.AppendLine("             <Place Name=\"has_left_1\" NumOfToken=\"0\" Capacity=\"0\">");
            sb.AppendLine("                 <Position X=\"1.6\" Y=\"1.7\" Width=\"0.2\" />");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"1.3\" Y=\"1.4\" Width=\"0.7\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("                 <Guard></Guard>");
            sb.AppendLine("             </Place>");

            sb.AppendLine("             <Place Name=\"has_right_1\" NumOfToken=\"0\" Capacity=\"0\">");
            sb.AppendLine("                 <Position X=\"1.6\" Y=\"3.7\" Width=\"0.2\" />");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"1.4\" Y=\"4\" Width=\"0.8\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("                 <Guard></Guard>");
            sb.AppendLine("             </Place>");

            sb.AppendLine("             <Place Name=\"fork0\" NumOfToken=\"1\" Capacity=\"0\">");
            sb.AppendLine("                 <Position X=\"4.3\" Y=\"3.7\" Width=\"0.2\" />");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"4.2\" Y=\"4\" Width=\"0.4\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("                 <Guard></Guard>");
            sb.AppendLine("             </Place>");

            sb.AppendLine("             <Place Name=\"fork1\" NumOfToken=\"1\" Capacity=\"0\">");
            sb.AppendLine("                 <Position X=\"4.3\" Y=\"1.7\" Width=\"0.2\" />");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"4.2\" Y=\"1.4\" Width=\"0.4\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("                 <Guard></Guard>");
            sb.AppendLine("             </Place>");

            sb.AppendLine("             <Place Name=\"full_1\" NumOfToken=\"0\" Capacity=\"0\">");
            sb.AppendLine("                 <Position X=\"2.7\" Y=\"2.7\" Width=\"0.2\" />");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"2.5\" Y=\"2.4\" Width=\"0.4\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("                 <Guard></Guard>");
            sb.AppendLine("             </Place>");

            sb.AppendLine("             <Place Name=\"full_0\" NumOfToken=\"0\" Capacity=\"0\">");
            sb.AppendLine("                 <Position X=\"5.9\" Y=\"2.7\" Width=\"0.2\" />");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"5.9\" Y=\"2.4\" Width=\"0.4\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("                 <Guard></Guard>");
            sb.AppendLine("             </Place>");
            sb.AppendLine("         </Places>");

            sb.AppendLine("         <Transitions>");
            sb.AppendLine("             <Transition Name=\"get_right_1\">");
            sb.AppendLine("                 <Position X=\"2.9\" Y=\"3.7\" Width=\"0.2\" />");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"2.7\" Y=\"4\" Width=\"0.8\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("                 <Guard></Guard>");
            sb.AppendLine("                 <Program></Program>");
            sb.AppendLine("             </Transition>");

            sb.AppendLine("             <Transition Name=\"get_left_0\">");
            sb.AppendLine("                 <Position X=\"5.7\" Y=\"3.7\" Width=\"0.2\" />");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"5.7\" Y=\"4\" Width=\"0.7\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("                 <Guard></Guard>");
            sb.AppendLine("                 <Program></Program>");
            sb.AppendLine("             </Transition>");

            sb.AppendLine("             <Transition Name=\"get_right_0\">");
            sb.AppendLine("                 <Position X=\"5.7\" Y=\"1.7\" Width=\"0.2\" />");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"5.5\" Y=\"1.4\" Width=\"0.8\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("                 <Guard></Guard>");
            sb.AppendLine("                 <Program></Program>");
            sb.AppendLine("             </Transition>");

            sb.AppendLine("             <Transition Name=\"get_left_1\">");
            sb.AppendLine("                 <Position X=\"2.9\" Y=\"1.7\" Width=\"0.2\" />");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"2.7\" Y=\"1.4\" Width=\"0.7\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("                 <Guard></Guard>");
            sb.AppendLine("                 <Program></Program>");
            sb.AppendLine("             </Transition>");

            sb.AppendLine("             <Transition Name=\"eat_1\">");
            sb.AppendLine("                 <Position X=\"1.6\" Y=\"2.7\" Width=\"0.2\" />");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"1.1\" Y=\"2.7\" Width=\"0.4\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("                 <Guard></Guard>");
            sb.AppendLine("                 <Program></Program>");
            sb.AppendLine("             </Transition>");

            sb.AppendLine("             <Transition Name=\"eat_0\">");
            sb.AppendLine("                 <Position X=\"6.8\" Y=\"2.7\" Width=\"0.2\" />");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"7.1\" Y=\"2.7\" Width=\"0.4\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("                 <Guard></Guard>");
            sb.AppendLine("                 <Program></Program>");
            sb.AppendLine("             </Transition>");

            sb.AppendLine("             <Transition Name=\"release_1\">");
            sb.AppendLine("                 <Position X=\"3.8\" Y=\"2.7\" Width=\"0.2\" />");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"3.4\" Y=\"2.4\" Width=\"0.7\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("                 <Guard></Guard>");
            sb.AppendLine("                 <Program></Program>");
            sb.AppendLine("             </Transition>");

            sb.AppendLine("             <Transition Name=\"release_0\">");
            sb.AppendLine("                 <Position X=\"4.9\" Y=\"2.74\" Width=\"0.2\" />");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"4.9\" Y=\"2.4\" Width=\"0.7\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("                 <Guard></Guard>");
            sb.AppendLine("                 <Program></Program>");
            sb.AppendLine("             </Transition>");
            sb.AppendLine("         </Transitions>");

            sb.AppendLine("         <Arcs>");
            sb.AppendLine("             <Arc From=\"has_left_1\" To=\"eat_1\" Weight=\"1\">");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"1.67\" Y=\"2.085\" Width=\"0.25\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("             </Arc>");

            sb.AppendLine("             <Arc From=\"has_right_1\" To=\"eat_1\" Weight=\"1\">");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"1.71\" Y=\"3.08\" Width=\"0.25\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("             </Arc>");

            sb.AppendLine("             <Arc From=\"fork1\" To=\"get_left_1\" Weight=\"1\">");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"3.6\" Y=\"1.6\" Width=\"0.25\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("             </Arc>");

            sb.AppendLine("             <Arc From=\"get_left_1\" To=\"has_left_1\" Weight=\"1\">");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"2.27\" Y=\"1.595\" Width=\"0.25\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("             </Arc>");

            sb.AppendLine("             <Arc From=\"fork0\" To=\"get_right_1\" Weight=\"1\">");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"3.7\" Y=\"3.6\" Width=\"0.25\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("             </Arc>");

            sb.AppendLine("             <Arc From=\"get_right_1\" To=\"has_right_1\" Weight=\"1\">");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"2.36\" Y=\"3.58\" Width=\"0.25\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("             </Arc>");

            sb.AppendLine("             <Arc From=\"fork0\" To=\"get_left_0\" Weight=\"1\">");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"5\" Y=\"3.6\" Width=\"0.25\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("             </Arc>");

            sb.AppendLine("             <Arc From=\"get_left_0\" To=\"has_left_0\" Weight=\"1\">");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"6.4\" Y=\"3.6\" Width=\"0.25\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("             </Arc>");

            sb.AppendLine("             <Arc From=\"has_left_0\" To=\"eat_0\" Weight=\"1\">");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"6.9\" Y=\"3.35\" Width=\"0.25\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("             </Arc>");

            sb.AppendLine("             <Arc From=\"has_right_0\" To=\"eat_0\" Weight=\"1\">");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"6.9\" Y=\"2.35\" Width=\"0.25\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("             </Arc>");

            sb.AppendLine("             <Arc From=\"fork1\" To=\"get_right_0\" Weight=\"1\">");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"5\" Y=\"1.6\" Width=\"0.25\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("             </Arc>");

            sb.AppendLine("             <Arc From=\"get_right_0\" To=\"has_right_0\" Weight=\"1\">");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"6.35\" Y=\"1.85\" Width=\"0.25\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("             </Arc>");

            sb.AppendLine("             <Arc From=\"eat_1\" To=\"full_1\" Weight=\"1\">");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"2.25\" Y=\"2.8\" Width=\"0.25\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("             </Arc>");

            sb.AppendLine("             <Arc From=\"full_1\" To=\"release_1\" Weight=\"1\">");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"3.35\" Y=\"2.8\" Width=\"0.25\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("             </Arc>");

            sb.AppendLine("             <Arc From=\"eat_0\" To=\"full_0\" Weight=\"1\">");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"6.35\" Y=\"2.8\" Width=\"0.25\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("             </Arc>");

            sb.AppendLine("             <Arc From=\"full_0\" To=\"release_0\" Weight=\"1\">");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"5.4\" Y=\"2.82\" Width=\"0.25\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("             </Arc>");

            sb.AppendLine("             <Arc From=\"release_1\" To=\"fork1\" Weight=\"1\">");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"4.15\" Y=\"2.3\" Width=\"0.25\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("             </Arc>");

            sb.AppendLine("             <Arc From=\"release_1\" To=\"fork0\" Weight=\"1\">");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"4.15\" Y=\"3.11\" Width=\"0.25\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("             </Arc>");

            sb.AppendLine("             <Arc From=\"release_0\" To=\"fork1\" Weight=\"1\">");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"4.6\" Y=\"2.3\" Width=\"0.25\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("             </Arc>");

            sb.AppendLine("             <Arc From=\"release_0\" To=\"fork0\" Weight=\"1\">");
            sb.AppendLine("                 <Label>");
            sb.AppendLine("                     <Position X=\"4.6\" Y=\"3.1\" Width=\"0.25\" />");
            sb.AppendLine("                 </Label>");
            sb.AppendLine("             </Arc>");
            sb.AppendLine("         </Arcs>");
            sb.AppendLine("     </Model>");
            sb.AppendLine(" </Models>");
            sb.AppendLine("</PN>");

            return sb.ToString();
        }
    }
}
