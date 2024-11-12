import requests

response = requests.options("http://localhost:5000/api/login", headers={
    "Origin": "http://localhost:8080",
    "Access-Control-Request-Method": "POST",
    "Access-Control-Request-Headers": "Content-Type"
})

print(response.headers)
print(response.status_code)
print(response.text)