# Medical Device Safety and Recall Notice Platform

A web-based platform for the company ANDtr to support employees carrying out risk analysis and review activities for medical devices. Employees can search a dataset of safety and recall notices, identify notices relevant to a risk review, and share findings with customers, who can view the information and comment.

Built with **ASP.NET Core MVC** (.NET 10) using **JetBrains Rider**.

## Users

| User | What they can do |
|---|---|
| **Employee** | Search recall notices, view notice details, select relevant notices, record findings, share with customers |
| **Customer** | View findings shared with them, read risk details, and comment on them |

## Features

**Employee**
- Search 616 FDA medical device recall notices (see `AndtrWeb/Data/recalls.csv`)
- Smarter search: ranked results, related-word matching (for example "heart" also finds cardiac), filters by recall class and status
- Paged results and a full notice details page

**Customer**
- Dashboard of findings shared with the customer, with status labels (Available, Expired)
- Review details page with risk level, recall reference, summary, potential risks and recommended actions
- Comments page showing the conversation in time order, with confirmation messages and an explanation when commenting is unavailable
- Customers can only open findings shared with them

## Project structure

```
AndtrWeb/
  Controllers/   CustomerController, EmployeeController
  Models/        SharedReview, RecallNotice, RecallRepository
  Views/         Customer/ (Dashboard, Details, Comments), Employee/ (Search, Notice)
  Data/          recalls.csv (cleaned FDA dataset)
  wwwroot/css/   customer.css
```

## Running the project

1. Open `AndtrWeb.sln` in JetBrains Rider (or Visual Studio) with the .NET 10 SDK installed.
2. Run the **AndtrWeb: http** configuration.
3. Open:
   - Customer dashboard: `/Customer/Dashboard`
   - Employee search: `/Employee/Search`

## Dataset

`Data/recalls.csv` is a cleaned version of the supplied FDA recall dataset (dates standardised to `yyyy-MM-dd`, line-break characters removed, an `Id` column added). FDA recall classes are shown as risk levels: Class 1 = High, Class 2 = Medium, Class 3 = Low.

## Status

Work in progress.

- Done: customer dashboard, review details, comments, employee search
- In progress: selecting notices, employee comments, storing findings, sharing with customers
- Planned: login with separate Employee and Customer roles, database storage, report generation

## Team

Team 04 (University software engineering project).
