# Multi CSV

#### Contents
- [Introduction](#intruduction)
- [Goals](#goals)
- [File Structure](#file-structure)
- [Example](#example)

## Intruduction
- Integrate multiple csv file into just one file.
- Implement with C#

## Goals
- The goal is saving multiple array on just one file regardless of its data type.

## File Structure
- Multi CSV file format repeats below pattern.
```
#<Alias_1>[,<Alias_2>] .. [,<Alias_N>]\n
<Head_1>[,<Head_2>] ... [,<Head_N>]\n
<Data_1>[,<Data_2>] ... [,<Data_N>]\n
\n
```
- Last `\n` character is essential as functionally.

## Example
- Let define class `Player`, `Item` and `Achivement` as below.
```
public class Player
{
  public string name;
  public int age;
  public int money;
}
```
```
public class Item
{
  public string name;
  public int count;
  public int grade;
}
```
```
public class Achivement
{
  public string title;
  public string description;
  public bool isCleared;
  public int difficulty;
}
```

- If we should save `T[]` or `List<T>`(any type of array) on file, the one idea is saving as `.csv` file format as below. let `T` is `Player`, `Item` and `Achivement`.
```
name,age,money
alice,12,1500
bob,13,2000
mallory,16,3200
```
```
name,count,grade
sword,1,4
dagger,1,3
pencil,3,9
```
```
title,description,isCleared,difficulty
Hello World!,,true,1
Bye World!,delete account,false,99
```
- This project suggests all arrays can integrate as below. (note: this document explicitly show new-line character.)
```
#Player\n
name,age,money\n
alice,12,1500\n
bob,13,2000\n
mallory,16,3200\n
\n
#Item\n
name,count,grade\n
sword,1,4\n
dagger,1,3\n
pencil,3,9\n
\n
#Achivement\n
title,description,isCleared,difficulty\n
Hello World!,,true,1\n
Bye World!,delete account,false,99\n
\n
```
