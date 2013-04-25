#########################################
# Makefile 
#########################################
bijou.exe: bijou.cs
	gmcs bijou.cs


videotron:
	mono bijou.exe -m -r:roche/Bijou

clean:
	rm bijou.exe


