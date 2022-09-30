# The Bible Code
 Searching skip code ciphers in Torah and other texts. CC-BY-SA-4.0
from [thebiblecode.sourceforge.io](https://thebiblecode.sourceforge.io) by [jeminacek](https://sourceforge.net/u/jeminacek/profile)

## Features
* Searching expressions in Torah
* Searching expressions in user's own texts
* Displaying results in matrix
* Searching other expressions in matrix
* Calculating probability of findings
* Displaying the statistical characteristics of text alphabeth
* Export to HTML

## How To Use

### Loading Your Own Text

Save your text into a text file. It does't have to be Hebrew text. Anything is fine as long as you use **UTF-8** encoding. Now go to **File -> Open text file** and select it. If you have opened your text before it can be found in the **adapted** folder in the program directory. In that case just use **File -> Open adapted file**

### Searching And Highlighting

Go to **File -> Search one or two expressions**. If you want to search just one, leave the second expression field empty. You can now select if you want to search in the Torah in in your own text.

Searching will do the following: It will search your first expression in any possible sequence of every n-th letter (eg. every 3rd letter, every 254th letter etc.). The maximum distance between letters is defined by the **Max skip** parameter.  
It will then write your expression into a column and fill the matrix with corresponding lines of the searched text. The expression will be in the centre and the matrix dimensions are specified by **Height** and **Width** parameters.  
The higher numbers you use the hight the chance of success but the longer time of searching.  
In case you have added a second expression, it will be searched for inside the matrix only.  
If you don't have Hebrew keyboard type your expressions by clicking at the letters in the **Alephbeth** box.

To stop the searching process, press **\[Escape\]**.

Now click on any result and see the corresponding matrix. Right clicking on expression 1 or 2 will offer you to add the expression into the **Legend**. In the Legend field you can change the colours. This is useful for the export.

### Export

Go to **File -> Export to html**.

### Printing

Go to **File -> Print**.

### Counting Probability

To prove that these so called "skip-code ciphers" are not an evidence of a divine origin of the Torah, a probability calculator was implemented. It basically shows you that the found results are purely random. You can search famous names or even your own name and you may succeed.  
To to that go to **Count -> Probability** and see that estimated number of results and the actual one are rarely much different.

### Tips

Some words are harder to find than others because they are too long while short words are too common and the searching will take incredibly long. Sometimes words are not found because the contained letters are not very frequent. To see how frequent letters in the chosen text are go to **Count -> Letter Frequency**.
