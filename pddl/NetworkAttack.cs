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
       
            GenerateDomain(sw);
            sw.Close();
        }

        private void GenerateDomain(StreamWriter sw)
        {
            sw.Write("(define \n");
            sw.Write("(domain " + Name + ")\n");
            sw.Write("(:types Machine Exploit OS)\n"); // not sure
            sw.Write(GenerateConstants() + "\n");
            sw.Write(GeneratePredicates());
            GenerateActions(sw);
            sw.Write(")");
        }

        private void GenerateActions(StreamWriter sw)
        {
            sw.WriteLine(GenerateInfectAction());
            sw.WriteLine(GeneratePing());

        }

        private string GeneratePing()
        {
            string sAction = "";

            sAction += "(:action ping\n";
            sAction += "\t:parameters (?x - Machine ?y - Machine ?s - OS)\n";
            sAction += "\t:precondition (connect ?x ?y) \n";
            sAction += "\t:observe (MachineOS ?y ?s)\n";
            sAction += ")\n";
            return sAction;
        }

        private string GenerateInfectAction()
        {
             string sAction ="";
           
                 sAction += "(:action infectOS\n";
                 sAction += "\t:parameters (?x - Machine ?y - Machine ?z - Exploit ?s - OS)\n";
                 sAction += "\t:precondition (connect ?x ?y) (infected ?x) (not(infected ?y)) (ExploitOS ?z ?s) (MachineOS ?y ?s) \n";
                 sAction += "\t:effect (infected ?y)\n";
            
                 sAction += ")\n";
           
            return sAction;
        }

        private string GeneratePredicates()
        {
            string sPredicates = "(:predicates\n";
            sPredicates += "\t(MachineOS ?x - Machine ?y - OS)\n";
            sPredicates += "\t(internal ?x - Machine)\n";
            sPredicates += "\t(connect ?x - Machine ?y - Machine)\n";
            sPredicates += "\t(infected ?x - Machine)\n";
            sPredicates += "\t(ExploitOS ?x - Exploit ?y - OS)\n";
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
            sConstants += " - OS\n";
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
            string sGoal = "(:goal (or";
            for (int iX = 1; iX <= Machines; iX++)
            {
                sGoal += "\t (and (infected " + inter(iX) + ") (internal " + inter(iX) + "))";
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
           
                for (int iy = 1; iy <= OS; iy++)
                {
                    sExploits += " (ExploitOS " + GetExploitOs(1, iy) + ")\n";


                }
                Random random = new Random();
    
                for (int iX = 2; iX <= Exploits; iX++)
                    for (int iy = 1; iy <= OS; iy++)
                    {
                        int randomNumber = random.Next(0, 100);
                        if (randomNumber % 4 == 0) //internal
                             sExploits += " (ExploitOS " + GetExploitOs(iX, iy) + ")\n";


                    }
            return sExploits;
        }

        private string GetInfected()
        {
            string sInfected = "";//"(and \n";
            //for each computer one OS
            //create link for all computers to all computers (Change it Later to random?)
     
                        sInfected += " (infected " + inter(1) + ")\n";


                
            return sInfected;
        }

        private string GetNetwork()
        {
            string sNetwork = "";//"(and \n";
            //for each computer one OS
            //create link for all computers to all computers (Change it Later to random?)
            //first-basic path
            sNetwork += " (connect " + connect(1, 2) + ")\n";
            sNetwork += " (connect " + connect(2, Machines) + ")\n";

            Random random = new Random();
            for (int iX = 1; iX <= Machines; iX++)
                for (int iy = 1; iy <= Machines; iy++)
                {
                    if ((iX < iy) & !(iX == 1 & iy == 2) & !(iX== 2 & iy == Machines))
                    {
                       
                        int randomNumber = random.Next(0, 100);
                        if (randomNumber %2==0) //internal
                            if (iX != iy)
                                sNetwork += " (connect " + connect(iX, iy) + ")\n";

                    }
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
                sNetwork = sNetwork + ")\n";
            }

            //sWaterPoisitions += ")\n";
            return sNetwork;
        }

        private string GetInternal()
        {
            string sInternal = "";//"(and \n";
            Random random = new Random();
            //for each computer one OS
            //create link for all computers to all computers (Change it Later to random?)
            for (int iX = 2; iX < Machines - 1; iX++)
            {
                int randomNumber = random.Next(0, 100);
                if (randomNumber %2 ==0) //internal

                    sInternal += " (internal " + inter(iX) + ")\n";


            }
            int last = Machines; //set at 1 internal 
            sInternal += " (internal " + inter(last) + ")";
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