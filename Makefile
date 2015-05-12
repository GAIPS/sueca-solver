CC=mcs
RR=mono
FILES = $(filter-out MainSuecaSolver.cs MainTest.cs MainWar.cs, $(wildcard *.cs))

GAMEMAIN=MainSuecaSolver.cs
TESTMAIN=MainTest.cs
WARMAIN=MainWar.cs

GAMEEXE=sueca-solver.exe
TESTEXE=test.exe
WAREXE=war.exe

game: clean compile-game run-game

test: clean compile-test run-test

war: clean compile-war run-war

compile-game:
	$(CC) -out:$(GAMEEXE) -reference:Microsoft.Solver.Foundation.dll -optimize+ $(GAMEMAIN) $(FILES)

compile-test:
	$(CC) -out:$(TESTEXE) -reference:Microsoft.Solver.Foundation.dll -optimize+ $(TESTMAIN) $(FILES)

compile-war:
	$(CC) -out:$(WAREXE) -reference:Microsoft.Solver.Foundation.dll -optimize+ $(WARMAIN) $(FILES)

run-game:
	time $(RR) $(GAMEEXE)

run-test:
	time $(RR) --profile=log $(TESTEXE)

run-war:
	time $(RR) $(WAREXE)

clean:
	rm -rf *.exe
