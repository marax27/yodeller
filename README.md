# Yodeller

[![.NET Build and Test](https://github.com/marax27/yodeller/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/marax27/yodeller/actions/workflows/build-and-test.yml)
[![Docker Image Version (tag latest semver)](https://img.shields.io/docker/v/marax27/yodeller)](https://hub.docker.com/r/marax27/yodeller)
[![Docker Image Size (latest semver)](https://img.shields.io/docker/image-size/marax27/yodeller)](https://hub.docker.com/r/marax27/yodeller)

Docker Hub: https://hub.docker.com/r/marax27/yodeller

## A simplistic UI wrapper on [yt-dlp](https://github.com/yt-dlp/yt-dlp)

Yodeller is a portable, batteries-included application. It allows you to easily download and store internet media (videos, audio).

Yodeller relies in particular on [yt-dlp](https://github.com/yt-dlp/yt-dlp) (called a _downloader_ in the UI) and [ffmpeg](https://ffmpeg.org/) (required for _post-processing_). Both dependencies are handled within the Docker image - they don't have to be installed on a host machine.

![Homepage screenshot](./docs/homepage-screenshot-01.png)

## Getting started

Run Yodeller instance using an image published on Docker Hub (please replace the `c:/where-to-store-downloaded-files` with an appropriate path on your machine):

    docker pull marax27/yodeller

    docker run -d -p 50500:80 -v c:/where-to-store-downloaded-files:/out marax27/yodeller

## Building the image yourself

Build...

    docker build -f ./src/Yodeller.Web/Dockerfile . -t yodeller

... and run (please replace the `c:/where-to-store-downloaded-files` with an appropriate path on your machine):

    docker run -d -p 50500:80 -v c:/where-to-store-downloaded-files:/out yodeller

## Technicalities

- .NET backend
- Simple WWW user interface
- Dockerised
