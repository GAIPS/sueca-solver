from pathlib import Path
import os.path
import math
import numpy as np
from sklearn import linear_model
import re

def main():
    # processedhandsFile = Path('sueca-logs/processedPlays.txt')
    numHands = 0
    abstractHands = {}
    numPlayFeatures = 0
    numHandFeatures = 0
    featuresName = []
    playClassification = ['0','2','3','4','10','11']
    numClasses = len(playClassification)
    
    # if processedhandsFile.is_file():
    if True:
        file = open('sueca-logs/processedPlays.txt','r') 
        firstLine = file.readline()
        splitLine = firstLine.split(',')
        numPlayFeatures = int(splitLine[0])
        numHandFeatures = int(splitLine[1])
        numSamples = int(splitLine[2])
        X = np.ones((numSamples, numHandFeatures )) #coef of 1 to w0
        # y = np.zeros((numSamples,numClasses))
        y = np.zeros(numSamples)

        secondLine = file.readline()
        splitLine = secondLine.split(',')
        for name in splitLine:
            featuresName.append(name)

        line = file.readline()
        i = 0
        while line != '\n':
            numHands += 1
            line = line.replace('\n','')
            splitLine = line.split(',')
            classification = splitLine[0]
            handFeatures = splitLine[1:]
            X[i] = handFeatures
            # classIndex = playClassification.index(classification)
            # y[i][classIndex] = 1
            y[i] = int(classification)
            line = file.readline()
            i += 1

        model = linear_model.SGDClassifier(loss='log')
        model.fit(X, y)
        print('Coefficients: \n', model.coef_)
        print("Mean squared error: %.2f" % np.mean((model.predict(X) - y) ** 2))
        #print('Variance score: %.2f' % regr.score(X, y))

        print(model.predict_proba(X))

        file.close()
    else:
        print('processedPlays file not found.')
        return


if __name__ == "__main__":
    main()
