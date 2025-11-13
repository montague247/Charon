FROM node:20 AS builder
WORKDIR /app
COPY package*.json ./
RUN npm install --ignore-scripts
RUN npm run build

FROM node:20-alpine AS runner
WORKDIR /app
COPY --from=builder /app/.next .next
COPY --from=builder /app/public public
COPY --from=builder /app/package*.json ./
RUN npm install  --ignore-scripts --production
EXPOSE 3000
USER nonroot
CMD ["npm", "start"]
