FROM node:alpine
WORKDIR /usr/expressapp
COPY ExpressApp/ .
RUN npm install

#RUN ./node_modules/.bin/tsc
CMD ["node", "./app.js"]