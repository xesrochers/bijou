#########################################
# Makefile 
#########################################
cs = src/Bijou.cs src/BijouUtils.cs src/MenuBuilder.cs src/BaseProcessor.cs src/HtmlProcessor.cs src/CsvProcessor.cs src/XmlProcessor.cs src/MdProcessor.cs src/RssProcessor.cs src/IcsProcessor.cs src/SearchProcessor.cs src/SubstitutionEngine.cs src/Watcher.cs 
bin/bijou.exe: $(cs)
	gmcs -out:bin/Bijou.exe $(cs)

local: 
	mono bin/Bijou.exe -m -w -i -r:roche/Bijou

pages:
	mono bin/Bijou.exe -m -r:roche/Bijou

clean:
	rm bin/Bijou.exe
	rm -R site/*




