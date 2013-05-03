.data
const_0: .asciz"Hello World!\n"
const_1: .asciz"%d %d\n"
const_2: .asciz"%d\n"
.text
.global _main
_main:
push %ebp
push %ebx
mov %esp,%ebp
sub $4,%esp
sub $4,%esp
mov $9,%ebx
mov %ebx,0(%esp)
mov $const_0,%ebx
mov %ebx,4(%esp)
mov %esp,%edx
sub $4,%edx
mov 4(%esp),%ebx
mov %ebx,0(%edx)
sub $4,%esp
call wypisz
add $4,%esp
mov %esp,%edx
sub $4,%edx
mov 0(%esp),%ebx
mov %ebx,0(%edx)
sub $4,%esp
call wypiszDwieLiczby
add $4,%esp
call inlineAsmExample
add $8,%esp
mov %ebp,%esp
pop %ebx
pop %ebp
mov $0,%eax
ret

.global wypisz
wypisz:
push %ebp
push %ebx
mov %esp,%ebp
mov %esp,%edx
sub $4,%edx
mov 12(%esp),%ebx
mov %ebx,0(%edx)
sub $4,%esp
call _printf
add $4,%esp
add $0,%esp
mov %ebp,%esp
pop %ebx
pop %ebp
mov $0,%eax
ret

.global wypiszDwieLiczby
wypiszDwieLiczby:
push %ebp
push %ebx
mov %esp,%ebp
sub $4,%esp
sub $4,%esp
mov $7,%ebx
mov %ebx,0(%esp)
mov $const_1,%ebx
mov %ebx,4(%esp)
mov %esp,%edx
sub $12,%edx
mov 20(%esp),%ebx
mov %ebx,8(%edx)
mov 0(%esp),%ebx
mov %ebx,4(%edx)
mov 4(%esp),%ebx
mov %ebx,0(%edx)
sub $12,%esp
call _printf
add $12,%esp
add $8,%esp
mov %ebp,%esp
pop %ebx
pop %ebp
mov $0,%eax
ret

.global inlineAsmExample
inlineAsmExample:
push %ebp
push %ebx
mov %esp,%ebp
sub $4,%esp
sub $4,%esp
mov $const_2,%ebx
mov %ebx,0(%esp)
mov $4,%ebx
mov %ebx,4(%esp)
movl $5, 4(%esp)
mov %esp,%edx
sub $8,%edx
mov 4(%esp),%ebx
mov %ebx,4(%edx)
mov 0(%esp),%ebx
mov %ebx,0(%edx)
sub $8,%esp
call _printf
add $8,%esp
add $8,%esp
mov %ebp,%esp
pop %ebx
pop %ebp
mov $0,%eax
ret

