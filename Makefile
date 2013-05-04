#########################################
# Makefile 
#########################################
bijou.exe: bijou.cs
	gmcs bijou.cs

bijou: 
	mono bijou.exe -m

videotron:
	mono bijou.exe -m -r:roche/Bijou

clean:
	rm bijou.exe


