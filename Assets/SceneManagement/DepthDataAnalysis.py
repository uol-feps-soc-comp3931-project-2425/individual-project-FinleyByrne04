import pandas as pd
import statsmodels.api as sm
from statsmodels.formula.api import ols

# Load the raw data
Data = pd.read_csv("DepthTaskResults.csv")

# Group by scene and environment to compute mean and std deviation
groupedData = Data.groupby(['SceneName', 'EnvironmentType'])
analysedData = groupedData['Error'].agg(['mean', 'std']).round(3)

# Print and save the calculated mean and std deviation
print("Mean and Standard Deviation of Errors:")
print(analysedData)
analysedData.to_csv("DepthTaskMean&Std.csv")

# Calculate individual ANOVA values, print and save
model_simple = ols('Error ~ C(SceneName) + C(EnvironmentType)', data=Data).fit()
anova_simple = sm.stats.anova_lm(model_simple, typ=2)

print("\nTwo-Way ANOVA (No Interaction):")
print(anova_simple)
anova_simple.to_csv("DepthTaskANOVASimple.csv")

# Find scenes that appear both indoors and outside
valid_scenes = Data.groupby('SceneName')['EnvironmentType'].nunique()
scenes_with_both_envs = valid_scenes[valid_scenes == 2].index # might not need this line
filtered_data = Data[Data['SceneName'].isin(scenes_with_both_envs)]

# Run ANOVA with interaction term on filtered data, print and save
model_interaction = ols('Error ~ C(SceneName) * C(EnvironmentType)', data=filtered_data).fit()
anova_interaction = sm.stats.anova_lm(model_interaction, typ=2)

print("\nTwo-Way ANOVA (With Interaction, Filtered Data):")
print(anova_interaction)
anova_interaction.to_csv("DepthTaskANOVAInteraction.csv")