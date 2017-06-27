CC=mcs
RR=mono
FILES = $(wildcard shared-files/*.cs)

GAMEMAIN=single-game/SingleGame.cs
TESTMAIN=test/Test.cs
WARMAIN=war/War.cs
STINFOMAIN=feature-extraction/Program.cs

GAMEEXE=simgle-game.exe
TESTEXE=test.exe
WAREXE=war.exe
STINFO=stinfo.exe

game: clean compile-game run-game

test: clean compile-test run-test

war: clean compile-war run-war

stinfo: clean compile-stinfo run-stinfo

compile-game:
	@echo '************  TEST VERSION ************'
	$(CC) -out:$(GAMEEXE) -reference:Microsoft.Solver.Foundation.dll -optimize+ $(GAMEMAIN) $(FILES)

compile-test:
	$(CC) -out:$(TESTEXE) -reference:Microsoft.Solver.Foundation.dll -optimize+ $(TESTMAIN) $(FILES)

compile-war:
	$(CC) -out:$(WAREXE) -reference:Microsoft.Solver.Foundation.dll -optimize+ $(WARMAIN) $(FILES)

compile-stinfo:
	$(CC) -out:$(STINFO) -reference:Microsoft.Solver.Foundation.dll -optimize+ $(STINFOMAIN) $(FILES)

run-game:
	time $(RR) $(GAMEEXE)

run-test:
	time $(RR) $(TESTEXE)

run-test-profiled:
	time $(RR) --gc=sgen --profile=log:noalloc $(TESTEXE)

run-war:
	time $(RR) $(WAREXE)

run-stinfo:
	time $(RR) $(STINFO)


clean:
	rm -rf *.exe
	rm -rf *.mlpd
