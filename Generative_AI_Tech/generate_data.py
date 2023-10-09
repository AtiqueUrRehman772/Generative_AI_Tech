# generate_data.py
import pandas as pd
import matplotlib.pyplot as plt

# Generate data (replace this with your data retrieval logic)
data = {
    'Year': [2021, 2022, 2023],
    'Salary': [50000, 55000, 60000]
}

df = pd.DataFrame(data)

# Generate a bar chart
plt.bar(df['Year'], df['Salary'])
plt.xlabel('Year')
plt.ylabel('Salary (USD)')
plt.title('Salary Trends')

# Save the chart as an image
plt.savefig('wwwroot/charts/salary_chart.png')

# Generate the HTML table
table_html = df.to_html(index=False)

with open('wwwroot/table.html', 'w') as table_file:
    table_file.write(table_html)
