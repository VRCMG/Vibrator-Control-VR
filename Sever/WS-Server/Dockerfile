FROM python:latest

RUN pip3 install pymongo requests websockets

WORKDIR /app

COPY ws-server.py ws-server.py
ENV PYTHONUNBUFFERED=1
CMD [ "python3", "ws-server.py" ]