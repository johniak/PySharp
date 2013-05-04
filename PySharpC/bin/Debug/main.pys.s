.data
const_0: .asciz"%s"
const_1: .asciz"tekst %d\n"
const_2: .asciz"Hello World!\n"
const_3: .asciz"%d"
const_4: .asciz"%d\n"
const_5: .asciz"%d %d\n"
const_6: .asciz"%d\n"
.text
.global _main
_main:
push %ebp
push %ebx
mov %esp,%ebp
sub $4,%esp #tekst
sub $4,%esp #liczba
mov $1024,%ebx
mov %ebx,0(%esp) #liczba
sub $4,%esp #buffor
sub $4,%esp #formater
mov $const_0,%ebx
mov %ebx,0(%esp) #formater
mov %esp,%edx
sub $8,%edx
mov $14,%ebx
mov %ebx,4(%edx)
mov $const_1,%ebx
mov %ebx,0(%edx)
sub $8,%esp
call _printf #^printf("tekst %d\n",14)
add $8,%esp
mov %esp,%edx
sub $4,%edx
mov 8(%esp),%ebx #liczba
mov %ebx,0(%edx)
sub $4,%esp
call _malloc #^malloc(liczba)
add $4,%esp
mov %eax,%ebx
mov %ebx,4(%esp) #buffor
mov %esp,%edx
sub $8,%edx
mov 4(%esp),%ebx #buffor
mov %ebx,4(%edx)
mov 0(%esp),%ebx #formater
mov %ebx,0(%edx)
sub $8,%esp
call _scanf #^scanf(formater,buffor)
add $8,%esp
mov %esp,%edx
sub $4,%edx
mov 4(%esp),%ebx #buffor
mov %ebx,0(%edx)
sub $4,%esp
call wypisz #wypisz(buffor)
add $4,%esp
mov $const_2,%ebx
mov %ebx,12(%esp) #tekst
mov %esp,%edx
sub $4,%edx
mov 12(%esp),%ebx #tekst
mov %ebx,0(%edx)
sub $4,%esp
call wypisz #wypisz(tekst)
add $4,%esp
mov %esp,%edx
sub $4,%edx
mov 8(%esp),%ebx #liczba
mov %ebx,0(%edx)
sub $4,%esp
call wypiszDwieLiczby #wypiszDwieLiczby(liczba)
add $4,%esp
call inlineAsmExample #inlineAsmExample()
call inputInt #inputInt()
add $16,%esp
mov %ebp,%esp
pop %ebx
pop %ebp
mov $0,%eax
ret

.global inputInt
inputInt:
push %ebp
push %ebx
mov %esp,%ebp
sub $4,%esp #liczba
sub $4,%esp #wsk
lea 4(%esp),%ebx
mov %ebx,0(%esp) #wsk
sub $4,%esp #formater
mov $const_3,%ebx
mov %ebx,0(%esp) #formater
mov %esp,%edx
sub $8,%edx
mov 4(%esp),%ebx #wsk
mov %ebx,4(%edx)
mov 0(%esp),%ebx #formater
mov %ebx,0(%edx)
sub $8,%esp
call _scanf #^scanf(formater,wsk)
add $8,%esp
mov $const_4,%ebx
mov %ebx,0(%esp) #formater
mov %esp,%edx
sub $8,%edx
mov 8(%esp),%ebx #liczba
mov %ebx,4(%edx)
mov 0(%esp),%ebx #formater
mov %ebx,0(%edx)
sub $8,%esp
call _printf #^printf(formater,liczba)
add $8,%esp
add $12,%esp
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
mov 12(%esp),%ebx #parametr
mov %ebx,0(%edx)
sub $4,%esp
call _printf #^printf(parametr)
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
sub $4,%esp #pattern
sub $4,%esp #liczba2
mov $7,%ebx
mov %ebx,0(%esp) #liczba2
mov $const_5,%ebx
mov %ebx,4(%esp) #pattern
mov %esp,%edx
sub $12,%edx
mov 20(%esp),%ebx #liczba1
mov %ebx,8(%edx)
mov 0(%esp),%ebx #liczba2
mov %ebx,4(%edx)
mov 4(%esp),%ebx #pattern
mov %ebx,0(%edx)
sub $12,%esp
call _printf #^printf(pattern,liczba2,liczba1)
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
sub $4,%esp #liczba
sub $4,%esp #pattern
mov $const_6,%ebx
mov %ebx,0(%esp) #pattern
mov $4,%ebx
mov %ebx,4(%esp) #liczba
movl $5, 4(%esp) #liczba
mov %esp,%edx
sub $8,%edx
mov 4(%esp),%ebx #liczba
mov %ebx,4(%edx)
mov 0(%esp),%ebx #pattern
mov %ebx,0(%edx)
sub $8,%esp
call _printf #^printf(pattern,liczba)
add $8,%esp
add $8,%esp
mov %ebp,%esp
pop %ebx
pop %ebp
mov $0,%eax
ret

