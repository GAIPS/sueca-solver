CC=mcs
FILES = $(wildcard *.cs)

all: lol

lol:
	$(CC) -out:sueca-solver.exe $(FILES)

clean:
	rm -rf *.exe