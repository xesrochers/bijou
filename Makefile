#########################################
# Makefile 
#########################################
bijou.exe: bijou.cs
	gmcs bijou.cs

clean:
	rm bijou.exe


