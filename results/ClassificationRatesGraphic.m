
y = [0 0 13.7931; 50.5845 59.0147 63.5021; 100 100 100];
b = bar(y)
set(gca,'XTickLabel', {'Hard', 'Medium', 'Easy'})
b(1).FaceColor = [0.2 0.4 0.2];
b(2).FaceColor = [0.2 0.5 1];
b(3).FaceColor = [1 0 0];
