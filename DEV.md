# Structure

Charon/
├── .github/
│   └── workflows/
│       └── ci-cd.yml         # GitHub Actions Pipeline
├── src/                      # .NET Projects
│   ├── Charon.Api/           # Web API project (.NET 8/9)
│   └── Charon.*/             # other Projects
│
├── frontend/                 # Next.js + React
│   ├── app/                  # Next.js App Router Pages
│   ├── components/           # React Components
│   ├── lib/                  # API-Clients, Hooks
│   ├── public/               # Static Assets
│   └── styles/               # Tailwind CSS
│
├── docker/                   # Dockerfiles for both parts
├── docker-compose.yml
└── README.md

# Frontend Setup
Inside the `frontend` folder:
```
npx create-next-app@latest .
```
Add styling:
```
npm install tailwindcss @tailwindcss/forms @tailwindcss/typography
```
Add API Helper:
```
npm install axios @tanstack/react-query
```

# Deployment
Secrets to set in the repo:
- AZURE_PUBLISH_PROFILE (wenn Azure genutzt wird → bekommst du im Azure Portal)
- oder AWS_ACCESS_KEY_ID + AWS_SECRET_ACCESS_KEY (wenn AWS ECS genutzt wird)
- optional DOCKERHUB_USERNAME + DOCKERHUB_TOKEN, falls du Docker Hub statt GHCR nutzt
