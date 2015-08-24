(define 
(problem NetworkAttack-3-2-2)
(:domain NetworkAttack-3-2-2)
(:init
(and
	(oneof (MachineOS M-1 OS-1) (MachineOS M-1 OS-2)
	(oneof (MachineOS M-2 OS-1) (MachineOS M-2 OS-2)
	(oneof (MachineOS M-3 OS-1) (MachineOS M-3 OS-2)

 (connect M-1 M-2)
 (connect M-1 M-3)
 (connect M-2 M-1)
 (connect M-2 M-3)
 (connect M-3 M-1)
 (connect M-3 M-2)

 (Internal M-3)
 (Infected M-1)

 (ExploitOS E-1 OS-1)
 (ExploitOS E-1 OS-2)
 (ExploitOS E-2 OS-1)
 (ExploitOS E-2 OS-2)

)
)

(:goal (and
	(or (Infected M-1) (and (Internal M-1)	(or (Infected M-2) (and (Internal M-2)	(or (Infected M-3) (and (Internal M-3)))

)
