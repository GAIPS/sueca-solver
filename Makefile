CC=mcs
RR=mono
FILES = $(wildcard *.cs)
EXEFILE=sueca-solver.exe

all: clean compile run

compile:
	$(CC) -out:$(EXEFILE) $(FILES)

run:
	$(RR) $(EXEFILE) 30

clean:
	rm -rf *.exe