PySharp
=======


Very simply language compiler (based on regexp). Code is compiled to native. 

Exemple of language:

  def main():
    ^printf("Hello world: %d \n",foo()*5)
  
  def foo():
    int wynik
    wynik=(3*3)%2
    ret wynik*3
