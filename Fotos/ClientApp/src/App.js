import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { UploadPhoto } from './components/UploadPhoto';
import { FetchPhotos } from './components/FetchPhotos';

import './custom.css'


export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
            <Route path='/upload-photo' component={UploadPhoto} />
            <Route path='/fetch-photos' component={FetchPhotos} />
      </Layout>
    );
  }
}


