# krastsvetmet
	2.2 Алгоритм Джонсона
Требуется за минимальное время обрабатывать n деталей, каждая i–ая деталь сначала обрабатывается на первом станке за время ai, а затем на вто-ром станке за bi время.
Джонсоном доказана оптимальность при минимальном времени рабо-ты производственной системы. Детали делят на 2 группы, у которых время обработки ai < bi. Сначала обрабатывают детали 1 группы в порядке воз-растания ai, а затем детали 2 группы в порядке убывания bi.
Развитием алгоритма Джонсона является задача о трех станках и m станках.
В рамках развития алгоритма Джонсона для m станков существуют 5 обобщений:
1.	В обработку запускаются детали с минимальным временем обработ-ки в порядке возрастания на 1 – ом станке, плюс скорейшее вовле-чение в обработку 2 – го станка. 
2.	В обработку запускаются детали с максимальным временем обра-ботки на последнем станке в порядке убывания, плюс уменьшается конечный простой 1 – го станка.
3.	В обработку запускаются детали, у которых «узкое» место находит-ся дальше от начала процесса обработки. 
Пояснение: «Узкое» место для детали – это станок, на котором обра-ботка детали занимает наибольшее время. 
4.	Вначале обрабатываются детали с максимальным суммарным вре-менем обработки на всех станках в порядке убывания.
5.	Производится усреднение результатов решения задач по четырем известным рекомендациям. Для этого для каждой детали ищется сумма мест во всех полученных решениях. Детали располагаются в порядке возрастания суммы мест.
При построении очереди стоит учитывать, что данные рекомендации несовместимы друг с другом, и построение очереди производится для каж-дого обобщения отдельно. В итоге получается 4 очереди запуска, используя которые, строится конечная очередь по пятому обобщению.

	2.3. Метод Петрова – Соколицына 
Петров и Соколицын установили, что в подавляющем большинстве случаев оптимальная последовательность работ будет наблюдаться в следу-ющих случаях:
1.	В случае распределения работ в порядке возрастания суммарно-го времени выполнения от первого до предпоследнего станка.
2.	В случае распределения работ в порядке убывания суммарного времени выполнения от второго до последнего станка.
3.	В порядке убывания разницы между временем выполнения на по-следнем и первом станке.
Таким образом, следует рассчитать две суммы и разность, по ним определить три возможные последовательности выполнения работ. Суммы минимизируют общее время обработки деталей, а разность минимизирует простой станков.
Срок окончания работ рассчитывается накопительным путем. Основа-ние ячейки = время обработки на станке + максимальное их двух значений (время освобождения станка от предыдущей детали, либо от детали). 
