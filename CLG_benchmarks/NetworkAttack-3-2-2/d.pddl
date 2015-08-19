(define 
(domain NetworkAttack-3-2-2)
(:types Machines)
(:constants M-1 M-2 M-3 - Machine
 OS-1 OS-2 - OperationSystem
 E-1 E-2 - Exploit
)

(:predicates
	(MachineOS ?x - Machine ?y - OperationSystem)
	(Internal ?x - Machine)
	(Connect ?x - Machine ?y - Machine)
	(Infected ?x - Machine)
	(ExploitOS ?x - Exploit ?y - OperationSystem)
)
(:action InfectOS0
	:parameters (?x - Machine ?y - Machine ?z - Exploit)
	:precondition (connect ?x ?y) (infected ?x) (not(infected ?y)) (ExploitOS ?z OS0) (MachineOS ?y OS0) 
	:effect (infected ?y)
)
(:action InfectOS1
	:parameters (?x - Machine ?y - Machine ?z - Exploit)
	:precondition (connect ?x ?y) (infected ?x) (not(infected ?y)) (ExploitOS ?z OS1) (MachineOS ?y OS1) 
	:effect (infected ?y)
)
(:action InfectOS2
	:parameters (?x - Machine ?y - Machine ?z - Exploit)
	:precondition (connect ?x ?y) (infected ?x) (not(infected ?y)) (ExploitOS ?z OS2) (MachineOS ?y OS2) 
	:effect (infected ?y)
)

)