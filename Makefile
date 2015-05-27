CC=dmcs
RR=mono
FILES = $(wildcard shared-game-items/*.cs)

GAMEMAIN=single-game/SingleGame.cs
TESTMAIN=test/Test.cs
WARMAIN=war/War.cs

GAMEEXE=simgle-game.exe
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
	time $(RR) $(TESTEXE)

run-test-profiled:
	time $(RR) --gc=sgen --profile=log:noalloc $(TESTEXE)

run-war:
	time $(RR) $(WAREXE)

clean:
	rm -rf *.exe
	rm -rf *.mlpd
