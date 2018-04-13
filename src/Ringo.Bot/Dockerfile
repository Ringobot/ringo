FROM node:carbon

# create app directory inside container
WORKDIR /usr/src/app

# npm install
COPY package*.json ./
RUN npm install
# in production RUN npm install --only=production

# Bundle app source
COPY . .

EXPOSE 3978

CMD ["npm", "start"]




