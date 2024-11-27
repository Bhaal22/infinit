.PHONY: build build-dev build-test run run-dev test

build:
	docker build --rm -t infinit .

build-dev:
	docker build --rm -t infinit.dev -f Dockerfile.dev .

build-test:
	docker build --rm -t infinit.tests -f Dockerfile.tests .

run:
	docker run --rm -e ACCESS_TOKEN=${ACCESS_TOKEN} -e OWNER=${OWNER} -e REPO=${REPO} infinit

run-dev:
	docker run --rm -e ACCESS_TOKEN=${ACCESS_TOKEN} -e OWNER=${OWNER} -e REPO=${REPO} -v `pwd`:/app -it infinit.dev

test:
	docker run -v `pwd`:/app -it infinit.tests
