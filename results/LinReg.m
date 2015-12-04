tbl = readtable('log2.txt', 'Delimiter','\t','ReadVariableNames',true,'ReadRowNames',false,'HeaderLines',1);
mdl = fitlm(tbl,'fp_02 ~ p_0 + p_2 + pt_0 + pt_2 + t_0 + t_2 + A_0 + A_2 + S_0 + S_2 + At_02 + suits_0 + suits_2 + first_0 + first_2')
result = table2array(mdl.Coefficients);
save('LinRegResult3.txt', 'result', '-ascii');