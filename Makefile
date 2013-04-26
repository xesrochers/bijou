#########################################
# Makefile 
#########################################
bijou.exe: bijou.cs
	gmcs bijou.cs

doc:
	mono bijou.exe -m

videotron:
	mono bijou.exe -m -r:roche/Bijou

clean:
	rm bijou.exe


