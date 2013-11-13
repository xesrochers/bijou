#########################################
# Makefile 
#########################################
cs = src/Bijou.cs src/BaseProcessor.cs src/HtmlProcessor.cs src/CsvProcessor.cs src/XmlProcessor.cs src/MdProcessor.cs src/RssProcessor.cs src/IcsProcessor.cs src/SearchProcessor.cs src/SubstitutionEngine.cs 
bin/bijou.exe: $(cs)
	gmcs $(cs)

bijou: 
	mono src/Bijou.exe -m -v

videotron:
	mono src/Bijou.exe -m -r:roche/Bijou

clean:
	rm src/Bijou.exe


