using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PDDL
{
    class NetworkAttack
    {
        private int Machines;
        private int OS;
        private int Exploits;

        public NetworkAttack(int cMachines, int cOS, int cExploits)
        {
            Machines = cMachines;
            OS = cOS;
            Exploits = cExploits;
        }

        public string Name { get { return "NetworkAttack-" + Machines + "-" + OS + "-" + Exploits; } }

        public void WriteDomain(string sPath)
        {
            if (!Directory.Exists(sPath))
                Directory.CreateDirectory(sPath);
            StreamWriter sw = new StreamWriter(sPath + @"\d.pddl");
            //sw.Write(GenerateDomain());
            GenerateDomain(sw);
            sw.Close();
        }

        private void GenerateDomain(StreamWriter sw)
        {
            sw.Write("(define \n");
            sw.Write("(domain " + Name + ")\n");
            sw.Write("(:types Machines)\n"); // not sure
            sw.Write(GenerateConstants() + "\n");
            sw.Write(GeneratePredicates());
            GenerateActions(sw);
            sw.Write(")");
        }

        private void GenerateActions(StreamWriter sw)
        {
            sw.WriteLine(GenerateInfectAction());

        }

        private string GenerateInfectAction()
        {
             string sAction ="";
             for (int i = 0; i <= OS; i++)
             {
                 sAction += "(:action InfectOS"+i+"\n";
                 sAction += "\t:parameters (?x - Machine ?y - Machine ?z - Exploit)\n";
                 sAction += "\t:precondition (connect ?x ?y) (infected ?x) (not(infected ?y)) (ExploitOS ?z OS" + i + ") (MachineOS ?y OS" + i + ") \n";
                 sAction += "\t:effect (infected ?y)\n";
            
                 sAction += ")\n";
             }
            return sAction;
        }

        private string GeneratePredicates()
        {
            string sPredicates = "(:predicates\n";
            sPredicates += "\t(MachineOS ?x - Machine ?y - OperationSystem)\n";
            sPredicates += "\t(Internal ?x - Machine)\n";
            sPredicates += "\t(Connect ?x - Machine ?y - Machine)\n";
            sPredicates += "\t(Infected ?x - Machine)\n";
            sPredicates += "\t(ExploitOS ?x - Exploit ?y - OperationSystem)\n";
            sPredicates += ")\n";
            return sPredicates;
        }

        private string GenerateConstants()
        {
            string sConstants = "(:constants";
            for (int i = 1; i <= Machines; i++)
                sConstants += " M-" + i;
            sConstants += " - Machine\n";
            for (int i = 1; i <= OS; i++)
                sConstants += " OS-" + i;
            sConstants += " - OperationSystem\n";
            for (int i =1; i <= Exploits; i++)
                sConstants += " E-" + i;
            sConstants += " - Exploit\n";

            sConstants += ")\n";
            return sConstants;
        }

        private string GenerateProblem()
        {

            string sProblem = "(define \n";
            sProblem += "(problem " + Name + ")\n";
            sProblem += "(:domain " + Name + ")\n";

            sProblem += GetInitState() + "\n";
            sProblem += GetGoalState() + "\n";
            sProblem += ")";
            return sProblem;
        }

        private string GetGoalState()
        {
            string sGoal = "(:goal (and\n";
            for (int iX = 1; iX <= Machines; iX++)
            {
                sGoal += "\t(or (Infected " + inter(iX) + ") (and (Internal " + inter(iX) + ")";
            }
            sGoal += "))\n";
            return sGoal;

        }

        private string GetInitState()
        {
            string sInit = "(:init\n";
            sInit += "(and\n";

            sInit += GetOSComputers() + "\n";
            sInit += GetNetwork() + "\n";
            sInit += GetInternal() + "\n";
            sInit += GetInfected()+ "\n";
            sInit += GetExploits() + "\n";
            sInit += ")\n)\n";
            return sInit;
        }

        private string GetExploits()
        {
            string sExploits = "";//"(and \n";
            //for each Exploit one OS
            //create link for all computers to all computers (Change it Later to random?)
            for (int iX = 1; iX <= Exploits; iX++)
                for (int iy = 1; iy <= OS; iy++)
                {
                    sExploits += " (ExploitOS " + GetExploitOs(iX, iy) + ")\n";


                }
            return sExploits;
        }

        private string GetInfected()
        {
            string sInfected = "";//"(and \n";
            //for each computer one OS
            //create link for all computers to all computers (Change it Later to random?)
     
                        sInfected += " (Infected " + inter(1) + ")\n";


                
            return sInfected;
        }

        private string GetNetwork()
        {
            string sNetwork = "";//"(and \n";
            //for each computer one OS
            //create link for all computers to all computers (Change it Later to random?)
            for (int iX = 1; iX <= Machines; iX++)
                for (int iy = 1; iy <= Machines; iy++)
                {
                    if (iX != iy)
                        sNetwork += " (connect " + connect(iX, iy) + ")\n";


                }
            return sNetwork;
        }

        private string GetOSComputers()
        {
            string sNetwork = "";//"(and \n";
            //for each computer one OS
            for (int iX = 1; iX <= Machines; iX++)
            {
                sNetwork += "\t(oneof";
                for (int iY = 1; iY <= OS; iY++)
                {

                    sNetwork += " (MachineOS " + GetMachineOs(iX, iY) + ")";

                }
                sNetwork = sNetwork + "\n";
            }

            //sWaterPoisitions += ")\n";
            return sNetwork;
        }

        private string GetInternal()
        {
            string sInternal = "";//"(and \n";

            //for each computer one OS
            //create link for all computers to all computers (Change it Later to random?)
            for (int iX = 2; iX < Machines - 1; iX++)
            {
                Random random = new Random();
                int randomNumber = random.Next(0, 100);
                if (randomNumber < 50) //internal

                    sInternal += " (Internal " + inter(iX) + ")\n";


            }
            int last = Machines; //set at 1 internal 
            sInternal += " (Internal " + inter(last) + ")";
            return sInternal;
        }

        private string inter(int iX)
        {
            return "M-" + iX;
        }

        private string connect(int iX, int iy)
        {
            return "M-" + iX + " M-" + iy;
        }

        private string GetMachineOs(int iX, int iY)
        {
            return "M-" + iX + " OS-" + iY;
        }

        private string GetExploitOs(int iX, int iY)
        {
            return "E-" + iX + " OS-" + iY;
        }

        public void WriteProblem(string sPath)
        {
            if (!Directory.Exists(sPath))
                Directory.CreateDirectory(sPath);
            StreamWriter sw = new StreamWriter(sPath + @"\p.pddl");
            sw.WriteLine(GenerateProblem());
            sw.Close();
        }
    }
}