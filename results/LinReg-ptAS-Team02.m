log = load('log1.txt');
pi_0 = log(:,1);
pi_2 = log(:,3);
ti_0 = log(:,5);
ti_2 = log(:,7);
Ai_0 = log(:,9);
Ai_2 = log(:,11);
Si_0 = log(:,13);
Si_2 = log(:,15);
pf_02 = log(:,17);
tbl = table(pi_0, pi_2, ti_0, ti_2, Ai_0, Ai_2, Si_0, Si_2, pf_02,'VariableNames',{'pi_0','pi_2','ti_0','ti_2','Ai_0','Ai_2','Si_0','Si_2','pf_02'});
mdl = fitlm(tbl,'pf_02 ~ pi_0 + pi_2 + ti_0 + ti_2 + Ai_0 + Ai_2 + Si_0 + Si_2')
result = table2array(mdl.Coefficients);
save('LinRegResult1.txt', 'result', '-ascii');
