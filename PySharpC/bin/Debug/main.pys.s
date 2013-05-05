.data
const_0: .asciz"przykladowy tekst %d \n"
.text
.global _main
_main:
push %ebp
push %ebx
mov %esp,%ebp
push %edx
call zwroc #zwroc()
mov %eax,%ebx
push %ebx #zwroc()
mov $5,%ebx
push %ebx #5
pop %ebx
pop %eax
clc
imul %ebx
push %eax
pop %ebx 
pop %edx
push %ebx
push %edx
mov $const_0,%ebx
pop %edx
push %ebx
call _printf #^printf("przykladowy tekst %d \n",zwroc()*5)
add $8,%esp
add $0,%esp
mov %ebp,%esp
pop %ebx
pop %ebp
mov $0,%eax
ret

.global zwroc
zwroc:
push %ebp
push %ebx
mov %esp,%ebp
sub $4,%esp #wynik
mov $3,%ebx
push %ebx #3
mov $3,%ebx
push %ebx #3
pop %ebx
pop %eax
clc
imul %ebx
push %eax
mov $2,%ebx
push %ebx #2
pop %ebx
pop %eax
clc
xor %edx,%edx
div %ebx
push %edx
pop %ebx 
mov %ebx,0(%esp) #wynik
mov 0(%esp),%ebx #wynik
push %ebx #wynik
mov $3,%ebx
push %ebx #3
pop %ebx
pop %eax
clc
imul %ebx
push %eax
pop %ebx 
mov %ebx,%eax
add $4,%esp
mov %ebp,%esp
pop %ebx
pop %ebp
ret
add $4,%esp
mov %ebp,%esp
pop %ebx
pop %ebp
mov $0,%eax
ret

