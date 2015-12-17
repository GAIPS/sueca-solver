// Filters

const notYou = p => p !== 'PM'
const wearsGlasses = p => p !== 'HC' && p !== 'JP' && p !== 'JA' && p !== 'RA'
const brownEyes = p => p !== 'BM'
const noBeard = p => p !== 'NM'
const cute = p => p !== 'MB'

// Secret Santa 2015
const people = ['BM', 'FC', 'HC', 'JP', 'JA', 'MB', 'NM', 'PM', 'RA']

const secretSanta = people.filter(notYou)
                          .filter(wearsGlasses)
                          .filter(brownEyes)
                          .filter(noBeard)
                          .filter(cute)

console.log(secretSanta)