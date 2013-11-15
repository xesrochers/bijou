#########################################
# Makefile 
#########################################
cs = src/Bijou.cs src/BaseProcessor.cs src/HtmlProcessor.cs src/CsvProcessor.cs src/XmlProcessor.cs src/MdProcessor.cs src/RssProcessor.cs src/IcsProcessor.cs src/SearchProcessor.cs src/SubstitutionEngine.cs 
bin/bijou.exe: $(cs)
	gmcs -out:bin/Bijou.exe $(cs)

bijou: 
	mono bin/Bijou.exe -m -v

videotron:
	mono bin/Bijou.exe -m -r:roche/Bijou

clean:
	rm bin/Bijou.exe
	rm -R site/*




