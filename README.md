# Flavoful Story

## Об игре
- Платформа: PC, Nintendo Switch

- Жанр: Аркадный социальный симулятор, Коузи, РПГ, Крафт, Исследование

- Стиль: 3D, Low-poly, PS2

- Движок: Unity

## Требования

 - [Требования по структуре папок](https://docs.google.com/document/d/1tSARu2g-VNt6iJd2riVN6J3Bq1rkdgX5/edit?usp=sharing&ouid=105076722265519793362&rtpof=true&sd=true)

- Требования по структуре папки Scripts:
    
    Папка должна разделяться на подпапки по ```namespace```. Каждый namespace должен представлять собой отдельную систему или модуль. Например, папка ```Saving``` включает в себя все классы / интерфейсы, связанные с системой сохранения. 
    
    Все классы / интерфейсы должны быть включены в соответствующий namespace. 
    
    Правило именования namespace: 

    ```ProjectName.Folder.[Subfolder].[FeatureName]```
    
    Например, ```namespace FlavorfulStory.Saving```