	.file	"main.c"
 # GNU C (GCC) version 4.7.2 (mingw32)
 #	compiled by GNU C version 4.7.2, GMP version 5.0.1, MPFR version 2.4.1, MPC version 0.8.1
 # GGC heuristics: --param ggc-min-expand=100 --param ggc-min-heapsize=131072
 # options passed:  -iprefix c:\mingw\bin\../lib/gcc/mingw32/4.7.2/ main.c
 # -mtune=i386 -march=i386 -fverbose-asm
 # options enabled:  -fasynchronous-unwind-tables -fauto-inc-dec
 # -fbranch-count-reg -fcommon -fdebug-types-section
 # -fdelete-null-pointer-checks -fdwarf2-cfi-asm -fearly-inlining
 # -feliminate-unused-debug-types -ffunction-cse -fgcse-lm -fgnu-runtime
 # -fident -finline-atomics -fira-share-save-slots -fira-share-spill-slots
 # -fivopts -fkeep-inline-dllexport -fkeep-static-consts
 # -fleading-underscore -fmath-errno -fmerge-debug-strings
 # -fmove-loop-invariants -fpeephole -fprefetch-loop-arrays
 # -freg-struct-return -fsched-critical-path-heuristic
 # -fsched-dep-count-heuristic -fsched-group-heuristic -fsched-interblock
 # -fsched-last-insn-heuristic -fsched-rank-heuristic -fsched-spec
 # -fsched-spec-insn-heuristic -fsched-stalled-insns-dep
 # -fset-stack-executable -fshow-column -fsigned-zeros
 # -fsplit-ivs-in-unroller -fstrict-volatile-bitfields -ftrapping-math
 # -ftree-cselim -ftree-forwprop -ftree-loop-if-convert -ftree-loop-im
 # -ftree-loop-ivcanon -ftree-loop-optimize -ftree-parallelize-loops=
 # -ftree-phiprop -ftree-pta -ftree-reassoc -ftree-scev-cprop
 # -ftree-slp-vectorize -ftree-vect-loop-version -funit-at-a-time
 # -funwind-tables -fvect-cost-model -fverbose-asm
 # -fzero-initialized-in-bss -m32 -m80387 -m96bit-long-double
 # -maccumulate-outgoing-args -malign-double -malign-stringops
 # -mfancy-math-387 -mfp-ret-in-387 -mieee-fp -mms-bitfields -mno-red-zone
 # -mno-sse4 -mpush-args -msahf -mstack-arg-probe

	.def	___main;	.scl	2;	.type	32;	.endef
	.text
	.globl	_main
	.def	_main;	.scl	2;	.type	32;	.endef
_main:
LFB0:
	.cfi_startproc
	pushl	%ebp	 #
	.cfi_def_cfa_offset 8
	.cfi_offset 5, -8
	movl	%esp, %ebp	 #,
	.cfi_def_cfa_register 5
	pushl	%ebx	 #
	andl	$-16, %esp	 #,
	subl	$16, %esp	 #,
	.cfi_offset 3, -12
	call	___main	 #
	movl	$20, 12(%esp)	 #, x
	movl	$5, 8(%esp)	 #, y
	movl	$7, 4(%esp)	 #, z
	movl	8(%esp), %eax	 # y, tmp63
	movl	12(%esp), %edx	 # x, tmp64
	leal	(%edx,%eax), %ecx	 #, D.1367
	movl	4(%esp), %edx	 # z, tmp65
	movl	%edx, %eax	 # tmp65, tmp66
	sall	$2, %eax	 #, tmp66
	addl	%edx, %eax	 # tmp65, tmp66
	sall	%eax	 # tmp66
	addl	%edx, %eax	 # tmp65, tmp66
	sall	$2, %eax	 #, tmp66
	addl	%edx, %eax	 # tmp65, tmp66
	movl	%eax, %edx	 # tmp66, tmp67
	sall	$5, %edx	 #, tmp67
	movl	%edx, %ebx	 # tmp67,
	subl	%eax, %ebx	 # tmp66,
	movl	%ebx, %eax	 #, D.1368
	addl	%ecx, %eax	 # D.1367, tmp68
	movl	%eax, (%esp)	 # tmp68, wynik
	movl	$2, %eax	 #, D.1369
	movl	-4(%ebp), %ebx	 #,
	leave
	.cfi_restore 5
	.cfi_restore 3
	.cfi_def_cfa 4, 4
	ret
	.cfi_endproc
LFE0:
