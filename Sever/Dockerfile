FROM python:latest

RUN pip3 install flask pymongo requests

WORKDIR /app

COPY server.py server.py
COPY index.html templates/index.html
ENV PYTHONUNBUFFERED=1
CMD [ "python3", "server.py" ]