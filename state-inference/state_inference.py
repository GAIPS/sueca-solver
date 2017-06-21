from pathlib import Path
import os.path
import math
import numpy as np
from sklearn import linear_model

def main():
    processedhandsFile = Path('..\sueca-logs\processedHands.txt')
    numHands = 0
    abstractHands = {}
    
    if processedhandsFile.is_file():
        file = open(processedhandsFile,'r') 

        for line in file:
            numHands += 1
            if line in abstractHands:
                abstractHands[line] += 1
            else:
                abstractHands[line] = 1

        file.close()
    else:
        print('ProcessedHands file not found.')
        return

    n_samples = len(abstractHands.keys())
    n_features = len(line.split('\t'))
    calculateFeaturesWeights(abstractHands, n_samples, n_features)

def calculateFeaturesWeights(hands, n_samples, n_features):
   
    X = np.ones((n_samples, n_features))
    y = np.ones(n_samples)
    i = 0
    for key in hands:
        y[i] = math.log(hands[key])
        parsed = key.split('\t')
        for j in range(n_features):
            if j < n_features - 1:
                X[i][j] = float(parsed[j])
        i += 1


    regr = linear_model.LinearRegression()
    regr.fit(X, y)
    print('Coefficients: \n', regr.coef_)
    print("Mean squared error: %.2f" % np.mean((regr.predict(X) - y) ** 2))
    print('Variance score: %.2f' % regr.score(X, y))


if __name__ == "__main__":
    main()
