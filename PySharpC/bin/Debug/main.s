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
	.section .rdata,"dr"
LC0:
	.ascii "%d %d %d\12\0"
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
	andl	$-16, %esp	 #,
	subl	$32, %esp	 #,
	call	___main	 #
	movl	$20, 28(%esp)	 #, x
	movl	$5, 24(%esp)	 #, y
	movl	$LC0, 20(%esp)	 #, pattern
	movl	24(%esp), %eax	 # y, tmp59
	movl	%eax, 8(%esp)	 # tmp59,
	movl	28(%esp), %eax	 # x, tmp60
	movl	%eax, 4(%esp)	 # tmp60,
	movl	20(%esp), %eax	 # pattern, tmp61
	movl	%eax, (%esp)	 # tmp61,
	call	_printf	 #
	movl	28(%esp), %eax	 # x, tmp62
	cmpl	24(%esp), %eax	 # y, tmp62
	jl	L2	 #,
	movl	28(%esp), %eax	 # x, tmp63
	cmpl	24(%esp), %eax	 # y, tmp63
	jle	L5	 #,
L2:
	movl	24(%esp), %eax	 # y, tmp64
	movl	%eax, 8(%esp)	 # tmp64,
	movl	28(%esp), %eax	 # x, tmp65
	movl	%eax, 4(%esp)	 # tmp65,
	movl	20(%esp), %eax	 # pattern, tmp66
	movl	%eax, (%esp)	 # tmp66,
	call	_printf	 #
L5:
	nop
L1:
	leave
	.cfi_restore 5
	.cfi_def_cfa 4, 4
	ret
	.cfi_endproc
LFE0:
	.def	_printf;	.scl	2;	.type	32;	.endef
